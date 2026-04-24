using System.Threading.Tasks;

namespace MobileRuntimePermissions.Internal
{
    internal interface IPlatformPermissionService
    {
        Task<PermissionInfo> GetInfoAsync(Permission permission);

        Task<PermissionStatus> GetStatusAsync(Permission permission);

        Task<PermissionStatus> RequestAsync(Permission permission);

        bool OpenAppSettings();

        bool OpenPermissionSettings(Permission permission);

        Task<PermissionInfo> GetAndroidPermissionInfoAsync(string fullPermissionName);

        Task<PermissionStatus> GetAndroidPermissionStatusAsync(string fullPermissionName);

        Task<PermissionStatus> RequestAndroidPermissionAsync(string fullPermissionName);
    }
}
