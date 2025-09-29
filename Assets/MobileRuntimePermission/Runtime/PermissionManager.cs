using MobileRuntimePermission.Core;

#if UNITY_ANDROID
using MobileRuntimePermission.Android;
#elif UNITY_IOS
using MobileRuntimePermission.iOS;
#endif

namespace MobileRuntimePermission
{
	public static class PermissionManager
	{
#if UNITY_ANDROID
		private static IPermissionManger _permissionManager = new AndroidPermissionsManager();
#elif UNITY_IOS
		private static readonly IPermissionManger _permissionManager = new IosPermissionManager();
#else
		private static readonly IPermissionManger _permissionManager = new NullablePermissionManager();
#endif

		public static PermissionRequestStatus PermissionStatus(Permission permission)
		{
			return _permissionManager.PermissionStatus(permission);
		}

		public static void RequestPermission(PermissionData permissionData)
		{
			_permissionManager.RequestPermission(permissionData);
		}
	}
}
