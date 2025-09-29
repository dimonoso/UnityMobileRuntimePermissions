#import <AVFoundation/AVFoundation.h>

@interface iOSRequestPermission : NSObject

@end

@implementation iOSRequestPermission

NSString* cameraPermissionName = @"CAMERA";
NSString* microphonePermissionName = @"RECORD_AUDIO";

+(int)permissionStatus:(char*)permission
{
    NSString* compareStr = [NSString stringWithUTF8String:permission];
    
    if ([compareStr isEqualToString:cameraPermissionName] == YES)
    {
        AVAuthorizationStatus status = [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo];
        if (status == AVAuthorizationStatusAuthorized)
        {
            return 1;
        }
        if (status == AVAuthorizationStatusNotDetermined)
        {
            return 0;
        }
        return -1;
    }
    if ([compareStr isEqualToString:microphonePermissionName] == YES)
    {
        AVAudioSessionRecordPermission status = [[AVAudioSession sharedInstance] recordPermission];
        if (status == AVAudioSessionRecordPermissionGranted)
        {
            return 1;
        }
        if (status == AVAudioSessionRecordPermissionUndetermined)
        {
            return 0;
        }
        return -1;
    }
    
    NSLog(@"permissionStatus ZEROO");
    return 0;
}

+(void)requestPermission:(const char*)permission
{
    NSString* compareStr = [NSString stringWithUTF8String:permission];
    
    if ([compareStr isEqualToString:cameraPermissionName] == YES)
    {
        const char *tmp = [cameraPermissionName UTF8String];
        
        [AVCaptureDevice requestAccessForMediaType:AVMediaTypeVideo completionHandler:^(BOOL granted) {
            if (granted)
            {
                [iOSRequestPermission callUnityObject:"IosPermission" Method:"PermissionGranted" Parameter:tmp];
            }
            else
            {
                [iOSRequestPermission callUnityObject:"IosPermission" Method:"PermissionDenied" Parameter:tmp];
            }
        }];
    }
    if ([compareStr isEqualToString:microphonePermissionName] == YES)
    {
        const char *tmp = [microphonePermissionName UTF8String];
        
        [[AVAudioSession sharedInstance] requestRecordPermission:^(BOOL granted) {
            if (granted)
            {
                [iOSRequestPermission callUnityObject:"IosPermission" Method:"PermissionGranted" Parameter:tmp];
            }
            else
            {
                [iOSRequestPermission callUnityObject:"IosPermission" Method:"PermissionDenied" Parameter:tmp];
            }
        }];
    }
}

+(void)callUnityObject:(const char*)object Method:(const char*)method Parameter:(const char*)parameter
{
    UnitySendMessage(object, method, parameter);
}

@end

extern "C"
{
    int _PermissionStatus(char *permission)
    {
        return [iOSRequestPermission permissionStatus:permission];
    }

    void _RequestPermission(char *permission)
    {
        [iOSRequestPermission requestPermission:permission];
    }
}
