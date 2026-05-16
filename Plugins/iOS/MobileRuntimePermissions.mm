#import <AVFoundation/AVFoundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
#import <CoreLocation/CoreLocation.h>
#import <Photos/Photos.h>
#import <UIKit/UIKit.h>
#import <UserNotifications/UserNotifications.h>

extern void UnitySendMessage(const char *obj, const char *method, const char *msg);

static NSString *MRPCallbackGameObject = @"MobileRuntimePermissionsiOSCallback";

static int MRPStatusAllow = 1;
static int MRPStatusDeny = 0;
static int MRPStatusDenyNeverAsk = -1;
static NSMutableDictionary<NSNumber *, id> *MRPLocationRequests;
static NSMutableDictionary<NSNumber *, id> *MRPBluetoothRequests;

static void MRPSendResult(int requestId, int status)
{
    NSString *payload = [NSString stringWithFormat:@"%d|%d", requestId, status];
    UnitySendMessage([MRPCallbackGameObject UTF8String], "OnPermissionResult", [payload UTF8String]);
}

static void MRPExecuteOnMainThread(dispatch_block_t block)
{
    if ([NSThread isMainThread])
    {
        block();
        return;
    }

    dispatch_sync(dispatch_get_main_queue(), block);
}

static NSString *MRPString(const char *value)
{
    if (value == NULL)
    {
        return @"";
    }

    return [NSString stringWithUTF8String:value];
}

static int MRPMapPhotoStatus(PHAuthorizationStatus status)
{
    switch (status)
    {
        case PHAuthorizationStatusAuthorized:
        case PHAuthorizationStatusLimited:
            return MRPStatusAllow;
        case PHAuthorizationStatusDenied:
        case PHAuthorizationStatusRestricted:
            return MRPStatusDenyNeverAsk;
        case PHAuthorizationStatusNotDetermined:
        default:
            return MRPStatusDeny;
    }
}

static int MRPMapLocationStatus(CLAuthorizationStatus status, BOOL requiresAlways)
{
    switch (status)
    {
        case kCLAuthorizationStatusAuthorizedAlways:
            return MRPStatusAllow;
        case kCLAuthorizationStatusAuthorizedWhenInUse:
            return requiresAlways ? MRPStatusDeny : MRPStatusAllow;
        case kCLAuthorizationStatusDenied:
        case kCLAuthorizationStatusRestricted:
            return MRPStatusDenyNeverAsk;
        case kCLAuthorizationStatusNotDetermined:
        default:
            return MRPStatusDeny;
    }
}

@interface MRPLocationRequester : NSObject<CLLocationManagerDelegate>
@property(nonatomic, assign) int requestId;
@property(nonatomic, assign) BOOL requestAlways;
@property(nonatomic, strong) CLLocationManager *manager;
@end

@implementation MRPLocationRequester

- (instancetype)initWithRequestId:(int)requestId requestAlways:(BOOL)requestAlways
{
    self = [super init];
    if (self != nil)
    {
        _requestId = requestId;
        _requestAlways = requestAlways;
        _manager = [[CLLocationManager alloc] init];
        _manager.delegate = self;
    }
    return self;
}

- (void)start
{
    if (self.requestAlways)
    {
        [self.manager requestAlwaysAuthorization];
    }
    else
    {
        [self.manager requestWhenInUseAuthorization];
    }
}

- (void)locationManager:(CLLocationManager *)manager didChangeAuthorizationStatus:(CLAuthorizationStatus)status
{
    MRPSendResult(self.requestId, MRPMapLocationStatus(status, self.requestAlways));
    [MRPLocationRequests removeObjectForKey:@(self.requestId)];
}

@end

@interface MRPBluetoothRequester : NSObject<CBCentralManagerDelegate>
@property(nonatomic, assign) int requestId;
@property(nonatomic, strong) CBCentralManager *manager;
@end

@implementation MRPBluetoothRequester

- (instancetype)initWithRequestId:(int)requestId
{
    self = [super init];
    if (self != nil)
    {
        _requestId = requestId;
    }
    return self;
}

- (void)start
{
    self.manager = [[CBCentralManager alloc] initWithDelegate:self queue:nil options:nil];
}

- (void)centralManagerDidUpdateState:(CBCentralManager *)centralManager
{
    if (@available(iOS 13.1, *))
    {
        switch (CBManager.authorization)
        {
            case CBManagerAuthorizationAllowedAlways:
                MRPSendResult(self.requestId, MRPStatusAllow);
                break;
            case CBManagerAuthorizationDenied:
            case CBManagerAuthorizationRestricted:
                MRPSendResult(self.requestId, MRPStatusDenyNeverAsk);
                break;
            case CBManagerAuthorizationNotDetermined:
            default:
                MRPSendResult(self.requestId, MRPStatusDeny);
                break;
        }
    }
    else
    {
        MRPSendResult(self.requestId, MRPStatusAllow);
    }

    [MRPBluetoothRequests removeObjectForKey:@(self.requestId)];
}

@end

extern "C"
{
    void MRP_Initialize(const char *gameObjectName)
    {
        NSString *value = MRPString(gameObjectName);
        if (value.length > 0)
        {
            MRPCallbackGameObject = value;
        }

        if (MRPLocationRequests == nil)
        {
            MRPLocationRequests = [[NSMutableDictionary alloc] init];
        }

        if (MRPBluetoothRequests == nil)
        {
            MRPBluetoothRequests = [[NSMutableDictionary alloc] init];
        }
    }

    int MRP_GetPermissionStatus(const char *permissionIdentifier)
    {
        NSString *permission = MRPString(permissionIdentifier);

        if ([permission isEqualToString:@"camera"])
        {
            AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
            if (status == AVAuthorizationStatusAuthorized)
            {
                return MRPStatusAllow;
            }

            if (status == AVAuthorizationStatusNotDetermined)
            {
                return MRPStatusDeny;
            }

            return MRPStatusDenyNeverAsk;
        }

        if ([permission isEqualToString:@"microphone"])
        {
            AVAudioSessionRecordPermission status = [[AVAudioSession sharedInstance] recordPermission];
            if (status == AVAudioSessionRecordPermissionGranted)
            {
                return MRPStatusAllow;
            }

            if (status == AVAudioSessionRecordPermissionUndetermined)
            {
                return MRPStatusDeny;
            }

            return MRPStatusDenyNeverAsk;
        }

        if ([permission isEqualToString:@"photos"])
        {
            return MRPMapPhotoStatus([PHPhotoLibrary authorizationStatusForAccessLevel:PHAccessLevelReadWrite]);
        }

        if ([permission isEqualToString:@"photos_add_only"])
        {
            return MRPMapPhotoStatus([PHPhotoLibrary authorizationStatusForAccessLevel:PHAccessLevelAddOnly]);
        }

        if ([permission isEqualToString:@"location_when_in_use"])
        {
            return MRPMapLocationStatus([CLLocationManager authorizationStatus], NO);
        }

        if ([permission isEqualToString:@"location_always"])
        {
            return MRPMapLocationStatus([CLLocationManager authorizationStatus], YES);
        }

        if ([permission isEqualToString:@"notifications"])
        {
            __block int result = MRPStatusDeny;
            dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);
            [[UNUserNotificationCenter currentNotificationCenter]
                getNotificationSettingsWithCompletionHandler:^(UNNotificationSettings *settings) {
                    if (settings.authorizationStatus == UNAuthorizationStatusAuthorized ||
                        settings.authorizationStatus == UNAuthorizationStatusProvisional ||
                        settings.authorizationStatus == UNAuthorizationStatusEphemeral)
                    {
                        result = MRPStatusAllow;
                    }
                    else if (settings.authorizationStatus == UNAuthorizationStatusDenied)
                    {
                        result = MRPStatusDenyNeverAsk;
                    }
                    else
                    {
                        result = MRPStatusDeny;
                    }

                    dispatch_semaphore_signal(semaphore);
                }];
            dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
            return result;
        }

        if ([permission isEqualToString:@"bluetooth"])
        {
            if (@available(iOS 13.1, *))
            {
                switch (CBManager.authorization)
                {
                    case CBManagerAuthorizationAllowedAlways:
                        return MRPStatusAllow;
                    case CBManagerAuthorizationDenied:
                    case CBManagerAuthorizationRestricted:
                        return MRPStatusDenyNeverAsk;
                    case CBManagerAuthorizationNotDetermined:
                    default:
                        return MRPStatusDeny;
                }
            }

            return MRPStatusAllow;
        }

        if ([permission isEqualToString:@"local_network"])
        {
            return MRPStatusDeny;
        }

        return MRPStatusDenyNeverAsk;
    }

    void MRP_RequestPermission(const char *permissionIdentifier, int requestId)
    {
        NSString *permission = MRPString(permissionIdentifier);

        if ([permission isEqualToString:@"camera"])
        {
            [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo
                                     completionHandler:^(BOOL granted) {
                                         MRPSendResult(requestId, granted ? MRPStatusAllow : MRPStatusDenyNeverAsk);
                                     }];
            return;
        }

        if ([permission isEqualToString:@"microphone"])
        {
            [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted) {
                MRPSendResult(requestId, granted ? MRPStatusAllow : MRPStatusDenyNeverAsk);
            }];
            return;
        }

        if ([permission isEqualToString:@"photos"])
        {
            [PHPhotoLibrary requestAuthorizationForAccessLevel:PHAccessLevelReadWrite
                                                       handler:^(PHAuthorizationStatus status) {
                                                           MRPSendResult(requestId, MRPMapPhotoStatus(status));
                                                       }];
            return;
        }

        if ([permission isEqualToString:@"photos_add_only"])
        {
            [PHPhotoLibrary requestAuthorizationForAccessLevel:PHAccessLevelAddOnly
                                                       handler:^(PHAuthorizationStatus status) {
                                                           MRPSendResult(requestId, MRPMapPhotoStatus(status));
                                                       }];
            return;
        }

        if ([permission isEqualToString:@"notifications"])
        {
            UNAuthorizationOptions options = UNAuthorizationOptionAlert | UNAuthorizationOptionBadge | UNAuthorizationOptionSound;
            [[UNUserNotificationCenter currentNotificationCenter]
                requestAuthorizationWithOptions:options
                              completionHandler:^(BOOL granted, NSError *error) {
                                  if (granted)
                                  {
                                      MRPSendResult(requestId, MRPStatusAllow);
                                  }
                                  else
                                  {
                                      MRPSendResult(requestId, MRPStatusDenyNeverAsk);
                                  }
                              }];
            return;
        }

        if ([permission isEqualToString:@"location_when_in_use"] ||
            [permission isEqualToString:@"location_always"])
        {
            BOOL requestAlways = [permission isEqualToString:@"location_always"];
            MRPLocationRequester *requester = [[MRPLocationRequester alloc] initWithRequestId:requestId requestAlways:requestAlways];
            MRPLocationRequests[@(requestId)] = requester;
            [requester start];
            return;
        }

        if ([permission isEqualToString:@"bluetooth"])
        {
            MRPBluetoothRequester *requester = [[MRPBluetoothRequester alloc] initWithRequestId:requestId];
            MRPBluetoothRequests[@(requestId)] = requester;
            [requester start];
            return;
        }

        if ([permission isEqualToString:@"local_network"])
        {
            MRPSendResult(requestId, MRPStatusDeny);
            return;
        }

        MRPSendResult(requestId, MRPStatusDenyNeverAsk);
    }

    bool MRP_OpenAppSettings(void)
    {
        __block BOOL opened = NO;
        MRPExecuteOnMainThread(^{
            NSURL *url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
            if ([[UIApplication sharedApplication] canOpenURL:url])
            {
                [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
                opened = YES;
            }
        });

        return opened;
    }

    bool MRP_OpenPermissionSettings(const char *permissionIdentifier)
    {
        NSString *permission = MRPString(permissionIdentifier);

        if ([permission isEqualToString:@"notifications"])
        {
            if (@available(iOS 16.0, *))
            {
                __block BOOL opened = NO;
                MRPExecuteOnMainThread(^{
                    NSURL *url = [NSURL URLWithString:UIApplicationOpenNotificationSettingsURLString];
                    if ([[UIApplication sharedApplication] canOpenURL:url])
                    {
                        [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
                        opened = YES;
                    }
                });

                if (opened)
                {
                    return YES;
                }
            }
        }

        return MRP_OpenAppSettings();
    }
}
