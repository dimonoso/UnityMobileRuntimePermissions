using MobileRuntimePermission.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MobileRuntimePermission.iOS
{
    public class IosPermissionManager : IPermissionManger
    {
        private Dictionary<string, PermissionData> _permissions = new Dictionary<string, PermissionData>();

        [DllImport("__Internal")]
        private static extern int _PermissionStatus(string permission);

        [DllImport("__Internal")]
        private static extern void _RequestPermission(string permission);

        public PermissionRequestStatus PermissionStatus(Permission permission)
        {
            var res = _PermissionStatus(permission.ToString());
            if (res == 1)
            {
                return PermissionRequestStatus.Allow;
            }

            if (res == -1)
            {
                return PermissionRequestStatus.DenyAndNeverAskAgain;
            }

            return PermissionRequestStatus.Deny;
        }

        public void RequestPermission(PermissionData permissionData)
        {
            _permissions[permissionData.Permission.ToString()] = permissionData;

            _RequestPermission(permissionData.Permission.ToString());
        }

        public IosPermissionManager()
        {
            var go = new GameObject();
            go.name = "IosPermission";
            GameObject.DontDestroyOnLoad(go);

            var callback = go.AddComponent<IosPermissionCallbackHandler>();
            callback.OnPermissionGranted += OnGranted;
            callback.OnPermissionDenied += OnDenied;
		}

        private void OnGranted(string permission)
        {
            Debug.Log("IosPermission OnGranted " + permission);
            if (_permissions.ContainsKey(permission))
            {
                Debug.Log("IosPermission OnAllow");
                _permissions[permission].OnAllow();
            }
        }

        private void OnDenied(string permission)
        {
            Debug.Log("IosPermission OnDenied");
            if (_permissions.ContainsKey(permission))
            {
                Debug.Log("IosPermission OnDeny");
                _permissions[permission].OnDeny();
            }
        }
    }
}
