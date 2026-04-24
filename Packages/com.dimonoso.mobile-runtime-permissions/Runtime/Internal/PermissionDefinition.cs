using System;

namespace MobileRuntimePermissions.Internal
{
    internal sealed class PermissionDefinition
    {
        public PermissionDefinition(
            Permission permission,
            string displayName,
            bool supportsAndroid,
            bool supportsIos,
            string iosNativeIdentifier,
            string[] iosUsageKeys,
            string[] androidPermissionNames,
            int androidRuntimeMinApi = 23)
        {
            Permission = permission;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            SupportsAndroid = supportsAndroid;
            SupportsIos = supportsIos;
            IosNativeIdentifier = iosNativeIdentifier ?? string.Empty;
            IosUsageKeys = iosUsageKeys ?? Array.Empty<string>();
            AndroidPermissionNames = androidPermissionNames ?? Array.Empty<string>();
            AndroidRuntimeMinApi = androidRuntimeMinApi;
        }

        public Permission Permission { get; }

        public string DisplayName { get; }

        public bool SupportsAndroid { get; }

        public bool SupportsIos { get; }

        public string IosNativeIdentifier { get; }

        public string[] IosUsageKeys { get; }

        public string[] AndroidPermissionNames { get; }

        public int AndroidRuntimeMinApi { get; }

        public bool IsCommon => !Permission.ToString().StartsWith("_", StringComparison.Ordinal);
    }
}
