using MobileRuntimePermission.Core;
using System;
using UnityEngine;

namespace MobileRuntimePermission.Android
{
	public class AndroidPermissionCallback : AndroidJavaProxy
	{
		public Action<int, PermissionRequestStatus> OnPermissionCallback;

		public AndroidPermissionCallback() : base(AndroidPermissionsManager.PACKAGE_NAME + "." + AndroidPermissionsManager.INTERFACE_NAME)
		{
		}

		void OnRequestPermissionsResult(int requestCode, int permissionState)
		{
			// 0 - Denied; 1 - Granted; -1 - DeniedNeverAsk
			var result = PermissionRequestStatus.Deny;
			if (permissionState == 0)
			{
				result = PermissionRequestStatus.Deny;
			}
			else if (permissionState == 1)
			{
				result = PermissionRequestStatus.Allow;
			}
			else if (permissionState == -1)
			{
				result = PermissionRequestStatus.DenyAndNeverAskAgain;
			}

			OnPermissionCallback?.Invoke(requestCode, result);
		}
	}
}
