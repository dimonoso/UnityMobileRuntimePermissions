#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MobileRuntimePermissions.Internal
{
    internal sealed class AndroidPermissionService : IPlatformPermissionService
    {
        private const string BridgeClassName = "com.dimonoso.mobileruntimepermissions.PermissionsBridge";
        private const int LegacyStorageRuntimeApi = 23;
        private const int ScopedStorageMediaApi = 33;

        private readonly AndroidJavaClass bridgeClass;
        private readonly Dictionary<int, TaskCompletionSource<PermissionStatus>> pendingRequests = new();
        private int nextRequestId = 1;

        public AndroidPermissionService()
        {
            AndroidPermissionCallbackReceiver.EnsureInstance();
            AndroidPermissionCallbackReceiver.PermissionResultReceived += OnPermissionResultReceived;

            bridgeClass = new AndroidJavaClass(BridgeClassName);
            bridgeClass.CallStatic("initialize", AndroidPermissionCallbackReceiver.GameObjectName);
        }

        public async Task<PermissionInfo> GetInfoAsync(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition))
            {
                return CreateUnsupportedPermissionInfo(permission, "Permission is not defined.");
            }

            if (!definition.SupportsAndroid)
            {
                return CreateUnsupportedPermissionInfo(permission, "Permission is not supported on Android.");
            }

            var permissionNames = ResolveAndroidPermissionNames(permission, definition);
            if (permissionNames.Length == 0)
            {
                return CreateUnsupportedPermissionInfo(permission, "Permission is not available for the current Android API level.");
            }

            var status = await GetAggregatedStatusAsync(permissionNames);
            return new PermissionInfo(
                permission,
                isSupported: true,
                canRequest: status == PermissionStatus.Deny,
                canOpenAppSettings: true,
                status: status,
                nativeIdentifier: string.Join(",", permissionNames),
                nativeDetails: $"Android SDK {GetAndroidSdkInt()}");
        }

        public async Task<PermissionStatus> GetStatusAsync(Permission permission)
        {
            var info = await GetInfoAsync(permission);
            return info.Status;
        }

        public async Task<PermissionStatus> RequestAsync(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition) || !definition.SupportsAndroid)
            {
                throw new PlatformNotSupportedException($"Permission '{permission}' is not supported on Android.");
            }

            var permissionNames = ResolveAndroidPermissionNames(permission, definition);
            if (permissionNames.Length == 0)
            {
                throw new PlatformNotSupportedException($"Permission '{permission}' is not available for the current Android API level.");
            }

            var results = new List<PermissionStatus>(permissionNames.Length);
            foreach (var permissionName in permissionNames)
            {
                results.Add(await RequestAndroidPermissionAsync(permissionName));
            }

            return AggregateStatuses(results);
        }

        public bool OpenAppSettings()
        {
            return bridgeClass.CallStatic<bool>("openAppSettings");
        }

        public bool OpenPermissionSettings(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition))
            {
                return OpenAppSettings();
            }

            var permissionNames = ResolveAndroidPermissionNames(permission, definition);
            var primaryPermission = permissionNames.FirstOrDefault() ?? string.Empty;
            return bridgeClass.CallStatic<bool>("openPermissionSettings", primaryPermission);
        }

        public async Task<PermissionInfo> GetAndroidPermissionInfoAsync(string fullPermissionName)
        {
            var status = await GetAndroidPermissionStatusAsync(fullPermissionName);
            return new PermissionInfo(
                Permission._AndroidLocationPrecise,
                isSupported: !string.IsNullOrWhiteSpace(fullPermissionName),
                canRequest: status == PermissionStatus.Deny,
                canOpenAppSettings: true,
                status: status,
                nativeIdentifier: fullPermissionName ?? string.Empty,
                nativeDetails: "Android raw permission");
        }

        public Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName)
        {
            if (string.IsNullOrWhiteSpace(fullPermissionName))
            {
                throw new ArgumentException("Permission name must not be empty.", nameof(fullPermissionName));
            }

            var statusValue = bridgeClass.CallStatic<int>("getPermissionStatus", fullPermissionName);
            return Task.FromResult((PermissionStatus)statusValue);
        }

        public async Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName)
        {
            if (string.IsNullOrWhiteSpace(fullPermissionName))
            {
                throw new ArgumentException("Permission name must not be empty.", nameof(fullPermissionName));
            }

            var currentStatus = await GetAndroidPermissionStatusAsync(fullPermissionName);
            if (currentStatus == PermissionStatus.Allow || currentStatus == PermissionStatus.DenyNeverAsk)
            {
                return currentStatus;
            }

            var requestId = nextRequestId++;
            var tcs = new TaskCompletionSource<PermissionStatus>();
            pendingRequests[requestId] = tcs;

            bridgeClass.CallStatic("requestPermission", fullPermissionName, requestId);
            return await tcs.Task;
        }

        private static PermissionInfo CreateUnsupportedPermissionInfo(Permission permission, string nativeDetails)
        {
            return new PermissionInfo(
                permission,
                isSupported: false,
                canRequest: false,
                canOpenAppSettings: false,
                status: PermissionStatus.DenyNeverAsk,
                nativeIdentifier: string.Empty,
                nativeDetails: nativeDetails);
        }

        private static PermissionStatus AggregateStatuses(IEnumerable<PermissionStatus> statuses)
        {
            var hasDeny = false;
            var hasNeverAsk = false;

            foreach (var status in statuses)
            {
                if (status == PermissionStatus.Deny)
                {
                    hasDeny = true;
                }
                else if (status == PermissionStatus.DenyNeverAsk)
                {
                    hasNeverAsk = true;
                }
            }

            if (hasDeny)
            {
                return PermissionStatus.Deny;
            }

            if (hasNeverAsk)
            {
                return PermissionStatus.DenyNeverAsk;
            }

            return PermissionStatus.Allow;
        }

        private async Task<PermissionStatus> GetAggregatedStatusAsync(IReadOnlyList<string> permissionNames)
        {
            var statuses = new List<PermissionStatus>(permissionNames.Count);
            foreach (var permissionName in permissionNames)
            {
                statuses.Add(await GetAndroidPermissionStatusAsync(permissionName));
            }

            return AggregateStatuses(statuses);
        }

        private static string[] ResolveAndroidPermissionNames(Permission permission, PermissionDefinition definition)
        {
            var sdk = GetAndroidSdkInt();
            switch (permission)
            {
                case Permission.Photos:
                    if (sdk >= ScopedStorageMediaApi)
                    {
                        return definition.AndroidPermissionNames;
                    }

                    if (sdk >= LegacyStorageRuntimeApi)
                    {
                        return new[] { "android.permission.READ_EXTERNAL_STORAGE" };
                    }

                    return Array.Empty<string>();

                case Permission.Notifications:
                    return sdk >= 33 ? definition.AndroidPermissionNames : Array.Empty<string>();

                case Permission.Bluetooth:
                case Permission._AndroidBluetoothScan:
                case Permission._AndroidBluetoothConnect:
                case Permission._AndroidBluetoothAdvertise:
                    return sdk >= 31 ? definition.AndroidPermissionNames : Array.Empty<string>();

                case Permission.LocalNetwork:
                case Permission._AndroidLocalNetwork:
                    return sdk >= 36 ? definition.AndroidPermissionNames : Array.Empty<string>();

                case Permission._AndroidReadMediaImages:
                case Permission._AndroidReadMediaVideo:
                case Permission._AndroidReadMediaAudio:
                    return sdk >= ScopedStorageMediaApi ? definition.AndroidPermissionNames : Array.Empty<string>();

                case Permission._AndroidReadMediaVisualUserSelected:
                    return sdk >= 34 ? definition.AndroidPermissionNames : Array.Empty<string>();

                case Permission._AndroidLocationBackground:
                    return sdk >= 29 ? definition.AndroidPermissionNames : Array.Empty<string>();

                default:
                    return sdk >= definition.AndroidRuntimeMinApi ? definition.AndroidPermissionNames : Array.Empty<string>();
            }
        }

        private static int GetAndroidSdkInt()
        {
            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT");
        }

        private void OnPermissionResultReceived(int requestId, PermissionStatus status)
        {
            if (!pendingRequests.TryGetValue(requestId, out var tcs))
            {
                return;
            }

            pendingRequests.Remove(requestId);
            tcs.TrySetResult(status);
        }
    }
}
#endif
