using System;
using UnityEngine;

namespace MobileRuntimePermission.iOS
{
    public class IosPermissionCallbackHandler : MonoBehaviour
    {
        public event Action<string> OnPermissionDenied;
        public event Action<string> OnPermissionGranted;

        private void PermissionGranted(string permission)
        {
            OnPermissionGranted?.Invoke(permission);
        }

        private void PermissionDenied(string permission)
        {
            OnPermissionDenied?.Invoke(permission);
        }
    }
}
