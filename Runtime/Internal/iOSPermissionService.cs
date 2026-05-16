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
                return CreateStatusPermissionInfo(permission, PermissionStatus.Unsupported, "Permission is not defined.");
            }

            if (!definition.SupportsIos || string.IsNullOrEmpty(definition.IosNativeIdentifier))
            {
                return CreateStatusPermissionInfo(permission, PermissionStatus.Unsupported, "Permission is not supported on iOS.");
            }

            var status = await GetStatusAsync(permission);
            return new PermissionInfo(
                permission,
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
                return Task.FromResult(PermissionStatus.Unsupported);
            }

            return Task.FromResult((PermissionStatus)MRP_GetPermissionStatus(definition.IosNativeIdentifier));
        }

        public async Task<PermissionStatus> RequestAsync(Permission permission)
        {
            if (!PermissionCatalog.TryGet(permission, out var definition) || !definition.SupportsIos)
            {
                return PermissionStatus.Unsupported;
            }

            var currentStatus = await GetStatusAsync(permission);
            if (currentStatus == PermissionStatus.Allow ||
                currentStatus == PermissionStatus.DenyNeverAsk ||
                currentStatus == PermissionStatus.Unsupported)
            {
                return currentStatus;
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
            return Task.FromResult(
                new PermissionInfo(
                    Permission.Camera,
                    canRequest: false,
                    canOpenAppSettings: false,
                    status: PermissionStatus.Unsupported,
                    nativeIdentifier: fullPermissionName ?? string.Empty,
                    nativeDetails: "Android raw permission API is not available on iOS."));
        }

        public Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName)
        {
            return Task.FromResult(PermissionStatus.Unsupported);
        }

        public Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName)
        {
            return Task.FromResult(PermissionStatus.Unsupported);
        }

        private static PermissionInfo CreateStatusPermissionInfo(
            Permission permission,
            PermissionStatus status,
            string nativeDetails)
        {
            return new PermissionInfo(
                permission,
                canRequest: status == PermissionStatus.Deny,
                canOpenAppSettings: status != PermissionStatus.Unsupported,
                status: status,
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
