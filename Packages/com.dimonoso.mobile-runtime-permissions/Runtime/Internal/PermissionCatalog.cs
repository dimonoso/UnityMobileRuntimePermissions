using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileRuntimePermissions.Internal
{
    internal static class PermissionCatalog
    {
        private static readonly Dictionary<Permission, PermissionDefinition> Definitions = new()
        {
            [Permission.Camera] = new PermissionDefinition(
                Permission.Camera,
                "Camera",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "camera",
                iosUsageKeys: new[] { "NSCameraUsageDescription" },
                androidPermissionNames: new[] { "android.permission.CAMERA" }),
            [Permission.Microphone] = new PermissionDefinition(
                Permission.Microphone,
                "Microphone",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "microphone",
                iosUsageKeys: new[] { "NSMicrophoneUsageDescription" },
                androidPermissionNames: new[] { "android.permission.RECORD_AUDIO" }),
            [Permission.Photos] = new PermissionDefinition(
                Permission.Photos,
                "Photos",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "photos",
                iosUsageKeys: new[] { "NSPhotoLibraryUsageDescription" },
                androidPermissionNames: new[] { "android.permission.READ_MEDIA_IMAGES", "android.permission.READ_MEDIA_VIDEO" },
                androidRuntimeMinApi: 33),
            [Permission.Location] = new PermissionDefinition(
                Permission.Location,
                "Location",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "location_when_in_use",
                iosUsageKeys: new[] { "NSLocationWhenInUseUsageDescription" },
                androidPermissionNames: new[] { "android.permission.ACCESS_FINE_LOCATION" }),
            [Permission.Notifications] = new PermissionDefinition(
                Permission.Notifications,
                "Notifications",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "notifications",
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.POST_NOTIFICATIONS" },
                androidRuntimeMinApi: 33),
            [Permission.Bluetooth] = new PermissionDefinition(
                Permission.Bluetooth,
                "Bluetooth",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "bluetooth",
                iosUsageKeys: new[] { "NSBluetoothAlwaysUsageDescription" },
                androidPermissionNames: new[] { "android.permission.BLUETOOTH_SCAN", "android.permission.BLUETOOTH_CONNECT" },
                androidRuntimeMinApi: 31),
            [Permission.LocalNetwork] = new PermissionDefinition(
                Permission.LocalNetwork,
                "Local Network",
                supportsAndroid: true,
                supportsIos: true,
                iosNativeIdentifier: "local_network",
                iosUsageKeys: new[] { "NSLocalNetworkUsageDescription" },
                androidPermissionNames: new[] { "android.permission.ACCESS_LOCAL_NETWORK" },
                androidRuntimeMinApi: 36),
            [Permission._AndroidLocationCoarse] = new PermissionDefinition(
                Permission._AndroidLocationCoarse,
                "Android Location (Coarse)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.ACCESS_COARSE_LOCATION" }),
            [Permission._AndroidLocationPrecise] = new PermissionDefinition(
                Permission._AndroidLocationPrecise,
                "Android Location (Precise)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.ACCESS_FINE_LOCATION" }),
            [Permission._AndroidLocationBackground] = new PermissionDefinition(
                Permission._AndroidLocationBackground,
                "Android Location (Background)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.ACCESS_BACKGROUND_LOCATION" },
                androidRuntimeMinApi: 29),
            [Permission._AndroidBluetoothScan] = new PermissionDefinition(
                Permission._AndroidBluetoothScan,
                "Android Bluetooth Scan",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.BLUETOOTH_SCAN" },
                androidRuntimeMinApi: 31),
            [Permission._AndroidBluetoothConnect] = new PermissionDefinition(
                Permission._AndroidBluetoothConnect,
                "Android Bluetooth Connect",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.BLUETOOTH_CONNECT" },
                androidRuntimeMinApi: 31),
            [Permission._AndroidBluetoothAdvertise] = new PermissionDefinition(
                Permission._AndroidBluetoothAdvertise,
                "Android Bluetooth Advertise",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.BLUETOOTH_ADVERTISE" },
                androidRuntimeMinApi: 31),
            [Permission._AndroidReadMediaImages] = new PermissionDefinition(
                Permission._AndroidReadMediaImages,
                "Android Media (Images)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.READ_MEDIA_IMAGES" },
                androidRuntimeMinApi: 33),
            [Permission._AndroidReadMediaVideo] = new PermissionDefinition(
                Permission._AndroidReadMediaVideo,
                "Android Media (Video)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.READ_MEDIA_VIDEO" },
                androidRuntimeMinApi: 33),
            [Permission._AndroidReadMediaAudio] = new PermissionDefinition(
                Permission._AndroidReadMediaAudio,
                "Android Media (Audio)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.READ_MEDIA_AUDIO" },
                androidRuntimeMinApi: 33),
            [Permission._AndroidReadMediaVisualUserSelected] = new PermissionDefinition(
                Permission._AndroidReadMediaVisualUserSelected,
                "Android Media (Selected Photos/Videos)",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.READ_MEDIA_VISUAL_USER_SELECTED" },
                androidRuntimeMinApi: 34),
            [Permission._AndroidLocalNetwork] = new PermissionDefinition(
                Permission._AndroidLocalNetwork,
                "Android Local Network",
                supportsAndroid: true,
                supportsIos: false,
                iosNativeIdentifier: string.Empty,
                iosUsageKeys: Array.Empty<string>(),
                androidPermissionNames: new[] { "android.permission.ACCESS_LOCAL_NETWORK" },
                androidRuntimeMinApi: 36),
            [Permission._iOSPhotoLibraryAddOnly] = new PermissionDefinition(
                Permission._iOSPhotoLibraryAddOnly,
                "iOS Photo Library (Add Only)",
                supportsAndroid: false,
                supportsIos: true,
                iosNativeIdentifier: "photos_add_only",
                iosUsageKeys: new[] { "NSPhotoLibraryAddUsageDescription" },
                androidPermissionNames: Array.Empty<string>()),
            [Permission._iOSLocationWhenInUse] = new PermissionDefinition(
                Permission._iOSLocationWhenInUse,
                "iOS Location (When In Use)",
                supportsAndroid: false,
                supportsIos: true,
                iosNativeIdentifier: "location_when_in_use",
                iosUsageKeys: new[] { "NSLocationWhenInUseUsageDescription" },
                androidPermissionNames: Array.Empty<string>()),
            [Permission._iOSLocationAlways] = new PermissionDefinition(
                Permission._iOSLocationAlways,
                "iOS Location (Always)",
                supportsAndroid: false,
                supportsIos: true,
                iosNativeIdentifier: "location_always",
                iosUsageKeys: new[] { "NSLocationWhenInUseUsageDescription", "NSLocationAlwaysAndWhenInUseUsageDescription" },
                androidPermissionNames: Array.Empty<string>()),
        };

        public static IReadOnlyCollection<PermissionDefinition> All => Definitions.Values.ToArray();

        public static bool TryGet(Permission permission, out PermissionDefinition definition)
        {
            return Definitions.TryGetValue(permission, out definition);
        }
    }
}
