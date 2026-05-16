using System.Threading.Tasks;
using MobileRuntimePermissions.Internal;

namespace MobileRuntimePermissions
{
    public static class PermissionManager
    {
        private static readonly IPlatformPermissionService Service = PlatformPermissionServiceFactory.Create();

        public static Task<PermissionInfo> GetInfoAsync(Permission permission)
        {
            return Service.GetInfoAsync(permission);
        }

        public static Task<PermissionStatus> GetStatusAsync(Permission permission)
        {
            return Service.GetStatusAsync(permission);
        }

        public static Task<PermissionStatus> RequestAsync(Permission permission)
        {
            return Service.RequestAsync(permission);
        }

        public static bool OpenAppSettings()
        {
            return Service.OpenAppSettings();
        }

        public static bool OpenPermissionSettings(Permission permission)
        {
            return Service.OpenPermissionSettings(permission);
        }

        public static Task<PermissionInfo> GetAndroidPermissionInfoAsync(string fullPermissionName)
        {
            return Service.GetAndroidPermissionInfoAsync(fullPermissionName);
        }

        public static Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName)
        {
            return Service.GetAndroidPermissionStatusAsync(fullPermissionName);
        }

        public static Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName)
        {
            return Service.RequestAndroidPermissionAsync(fullPermissionName);
        }
    }
}
