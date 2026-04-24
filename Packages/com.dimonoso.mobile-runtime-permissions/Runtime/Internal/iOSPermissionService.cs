#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MobileRuntimePermissions.Internal
{
    internal sealed class iOSPermissionService : IPlatformPermissionService
    {
        private readonly Dictionary<int, TaskCompletionSource<PermissionStatus>> pendingRequests = new();
        private int nextRequestId = 1;

        public iOSPermissionService()
        {
            iOSPermissionCallbackReceiver.EnsureInstance();
            iOSPermissionCallbackReceiver.PermissionResultReceived += OnPermissionResultReceived;
            MRP_Initialize(iOSPermissionCallbackReceiver.GameObjectName);
        }

        public async Task<PermissionInfo> GetInfoAsync(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition))
            {
                return CreateUnsupportedPermissionInfo(permission, "Permission is not defined.");
            }

            if (!definition.SupportsIos || string.IsNullOrEmpty(definition.IosNativeIdentifier))
            {
                return CreateUnsupportedPermissionInfo(permission, "Permission is not supported on iOS.");
            }

            var status = await GetStatusAsync(permission);
            return new PermissionInfo(
                permission,
                isSupported: true,
                canRequest: status == PermissionStatus.Deny,
                canOpenAppSettings: true,
                status: status,
                nativeIdentifier: definition.IosNativeIdentifier,
                nativeDetails: string.Join(",", definition.IosUsageKeys));
        }

        public Task<PermissionStatus> GetStatusAsync(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition) || !definition.SupportsIos)
            {
                return Task.FromResult(PermissionStatus.DenyNeverAsk);
            }

            return Task.FromResult((PermissionStatus)MRP_GetPermissionStatus(definition.IosNativeIdentifier));
        }

        public Task<PermissionStatus> RequestAsync(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition) || !definition.SupportsIos)
            {
                throw new PlatformNotSupportedException($"Permission '{permission}' is not supported on iOS.");
            }

            var requestId = nextRequestId++;
            var tcs = new TaskCompletionSource<PermissionStatus>();
            pendingRequests[requestId] = tcs;

            MRP_RequestPermission(definition.IosNativeIdentifier, requestId);
            return tcs.Task;
        }

        public bool OpenAppSettings()
        {
            return MRP_OpenAppSettings();
        }

        public bool OpenPermissionSettings(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition))
            {
                return OpenAppSettings();
            }

            return MRP_OpenPermissionSettings(definition.IosNativeIdentifier);
        }

        public Task<PermissionInfo> GetAndroidPermissionInfoAsync(string fullPermissionName)
        {
            throw new PlatformNotSupportedException("Android raw permission API is not available on iOS.");
        }

        public Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName)
        {
            throw new PlatformNotSupportedException("Android raw permission API is not available on iOS.");
        }

        public Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName)
        {
            throw new PlatformNotSupportedException("Android raw permission API is not available on iOS.");
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

        private void OnPermissionResultReceived(int requestId, PermissionStatus status)
        {
            if (!pendingRequests.TryGetValue(requestId, out var tcs))
            {
                return;
            }

            pendingRequests.Remove(requestId);
            tcs.TrySetResult(status);
        }

        [DllImport("__Internal")]
        private static extern void MRP_Initialize(string gameObjectName);

        [DllImport("__Internal")]
        private static extern int MRP_GetPermissionStatus(string permissionIdentifier);

        [DllImport("__Internal")]
        private static extern void MRP_RequestPermission(string permissionIdentifier, int requestId);

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MRP_OpenAppSettings();

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool MRP_OpenPermissionSettings(string permissionIdentifier);
    }
}
#endif
