using System;
using System.Threading.Tasks;

namespace MobileRuntimePermissions.Internal
{
    internal sealed class UnsupportedPermissionService : IPlatformPermissionService
    {
        public Task<PermissionInfo> GetInfoAsync(Permission permission)
        {
            return Task.FromResult(
                new PermissionInfo(
                    permission,
                    canRequest: false,
                    canOpenAppSettings: false,
                    status: PermissionStatus.Unsupported,
                    nativeIdentifier: string.Empty,
                    nativeDetails: "Current platform is not supported."));
        }

        public Task<PermissionStatus> GetStatusAsync(Permission permission)
        {
            return Task.FromResult(PermissionStatus.Unsupported);
        }

        public Task<PermissionStatus> RequestAsync(Permission permission)
        {
            return Task.FromResult(PermissionStatus.Unsupported);
        }

        public bool OpenAppSettings()
        {
            return false;
        }

        public bool OpenPermissionSettings(Permission permission)
        {
            return false;
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
                    nativeDetails: "Android raw permission API is only available on Android."));
        }

        public Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName)
        {
            return Task.FromResult(PermissionStatus.Unsupported);
        }

        public Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName)
        {
            return Task.FromResult(PermissionStatus.Unsupported);
        }
    }
}
