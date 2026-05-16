#if UNITY_IOS
using System;
using UnityEngine;

namespace MobileRuntimePermissions.Internal
{
    internal sealed class iOSPermissionCallbackReceiver : MonoBehaviour
    {
        public const string GameObjectName = "MobileRuntimePermissionsiOSCallback";

        public static event Action<int, PermissionStatus> PermissionResultReceived;

        public static iOSPermissionCallbackReceiver EnsureInstance()
        {
            var existing = FindFirstObjectByType<iOSPermissionCallbackReceiver>();
            if (existing != null)
            {
                return existing;
            }

            var gameObject = new GameObject(GameObjectName);
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<iOSPermissionCallbackReceiver>();
        }

        public void OnPermissionResult(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                return;
            }

            var parts = payload.Split('|');
            if (parts.Length != 2)
            {
                return;
            }

            if (!int.TryParse(parts[0], out var requestId))
            {
                return;
            }

            if (!int.TryParse(parts[1], out var statusValue))
            {
                return;
            }

            PermissionResultReceived?.Invoke(requestId, (PermissionStatus)statusValue);
        }
    }
}
#endif
