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
                    isSupported: false,
                    canRequest: false,
                    canOpenAppSettings: false,
                    status: PermissionStatus.DenyNeverAsk,
                    nativeIdentifier: string.Empty,
                    nativeDetails: "Current platform is not supported."));
        }

        public Task<PermissionStatus> GetStatusAsync(Permission permission)
        {
            return Task.FromResult(PermissionStatus.DenyNeverAsk);
        }

        public Task<PermissionStatus> RequestAsync(Permission permission)
        {
            throw new PlatformNotSupportedException($"Permission '{permission}' is not supported on this platform.");
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
                    isSupported: false,
                    canRequest: false,
                    canOpenAppSettings: false,
                    status: PermissionStatus.DenyNeverAsk,
                    nativeIdentifier: fullPermissionName ?? string.Empty,
                    nativeDetails: "Android raw permission API is only available on Android."));
        }

        public Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName)
        {
            return Task.FromResult(PermissionStatus.DenyNeverAsk);
        }

        public Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName)
        {
            throw new PlatformNotSupportedException("Android raw permission API is only available on Android.");
        }
    }
}
