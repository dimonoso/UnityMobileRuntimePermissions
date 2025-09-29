namespace MobileRuntimePermission.Core
{
    public interface IPermissionManger
    {
		PermissionRequestStatus PermissionStatus(Permission permission);
		void RequestPermission(PermissionData permissionData);
	}
}
