using System.Threading.Tasks;
using UnityEngine;

namespace MobileRuntimePermissions.Samples
{
    public sealed class PermissionSample : MonoBehaviour
    {
        [SerializeField]
        private Permission permission = Permission.Camera;

        public async void RequestSelectedPermission()
        {
            var status = await PermissionManager.RequestAsync(permission);
            Debug.Log($"Permission '{permission}' => {status}");
        }

        public async void RefreshStatus()
        {
            var info = await PermissionManager.GetInfoAsync(permission);
            Debug.Log(
                $"Permission '{permission}' status={info.Status} canRequest={info.CanRequest}");
        }

        public void OpenSettings()
        {
            PermissionManager.OpenPermissionSettings(permission);
        }

        private async void Start()
        {
            await Task.Yield();
            await PermissionManager.GetInfoAsync(permission);
        }
    }
}
