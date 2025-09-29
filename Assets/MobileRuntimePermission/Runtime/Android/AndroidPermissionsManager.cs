using MobileRuntimePermission.Core;
using System.Collections.Generic;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
#endif

namespace MobileRuntimePermission.Android
{
	public class AndroidPermissionsManager : IPermissionManger
	{
		public const string CLASS_NAME = "UnityPermissionActivity";
		public const string INTERFACE_NAME = "PermissionCallback";
		public const string PACKAGE_NAME = "com.dimonoso.unitypermission";

		private int _requestCode = 0;

		private Dictionary<int, PermissionData> _permissions = new Dictionary<int, PermissionData>();

		public AndroidPermissionsManager()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			var callbackListener = new AndroidPermissionCallback();
			callbackListener.OnPermissionCallback += OnPermissionRequest;

			var service = new AndroidJavaClass(PACKAGE_NAME + "." + CLASS_NAME);
			service.CallStatic("initPermissionCallback", callbackListener);
#endif
		}

		public PermissionRequestStatus PermissionStatus(Permission permission)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
		using( var androidUtils = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
		{
			var permissionState = androidUtils.GetStatic<AndroidJavaObject>( "currentActivity" ).Call<int>( "getPermissionState", GetPermissionStr( permission ) );
			var result = PermissionRequestStatus.Deny;
			if (permissionState == 1)
			{
				result = PermissionRequestStatus.Allow;
			}
			else if (permissionState == -1)
			{
				result = PermissionRequestStatus.DenyAndNeverAskAgain;
			}
			return result;
		}
#else
			return PermissionRequestStatus.Allow;
#endif
		}

		public void RequestPermission(PermissionData permissionData)
		{
			_permissions.Add(_requestCode, permissionData);

#if UNITY_ANDROID && !UNITY_EDITOR
		using ( var androidUtils = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
		{
			androidUtils.GetStatic<AndroidJavaObject>( "currentActivity" ).Call( "requestPermission", GetPermissionStr(permissionData.Permission), _requestCode );
		}
#endif

			_requestCode++;
		}

		private static string GetPermissionStr(Permission permission)
		{
			return "android.permission." + permission.ToString();
		}

		private void OnPermissionRequest(int requestCode, PermissionRequestStatus permissionState)
		{
			if (!_permissions.ContainsKey(requestCode))
			{
				return;
			}
			if (permissionState == PermissionRequestStatus.Allow)
			{
				_permissions[requestCode].OnAllow?.Invoke();
			}
			else if (permissionState == PermissionRequestStatus.Deny)
			{
				_permissions[requestCode].OnDeny?.Invoke();
			}
			else
			{
				_permissions[requestCode].OnDenyAndNeverAskAgain?.Invoke();
			}

			_permissions.Remove(_requestCode);
		}
	}
}
