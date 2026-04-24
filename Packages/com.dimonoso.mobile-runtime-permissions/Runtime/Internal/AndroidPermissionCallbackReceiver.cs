#if UNITY_ANDROID
using System;
using UnityEngine;

namespace MobileRuntimePermissions.Internal
{
    internal sealed class AndroidPermissionCallbackReceiver : MonoBehaviour
    {
        public const string GameObjectName = "MobileRuntimePermissionsAndroidCallback";

        public static event Action<int, PermissionStatus> PermissionResultReceived;

        public static AndroidPermissionCallbackReceiver EnsureInstance()
        {
            var existing = FindFirstObjectByType<AndroidPermissionCallbackReceiver>();
            if (existing != null)
            {
                return existing;
            }

            var gameObject = new GameObject(GameObjectName);
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<AndroidPermissionCallbackReceiver>();
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
