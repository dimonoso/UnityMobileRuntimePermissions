using MobileRuntimePermission.Core;

namespace MobileRuntimePermission
{
	public class NullablePermissionManager : IPermissionManger
	{
		public PermissionRequestStatus PermissionStatus(Permission permission)
		{
			return PermissionRequestStatus.Allow;
		}

		public void RequestPermission(PermissionData permissionData)
		{
			permissionData?.OnAllow();
		}
	}
}
