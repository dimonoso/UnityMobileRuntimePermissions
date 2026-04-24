using System.Linq;
using MobileRuntimePermissions.Internal;
using MobileRuntimePermissions.Editor.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace MobileRuntimePermissions.Editor.Build
{
    internal sealed class MobileRuntimePermissionsBuildValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != UnityEditor.BuildTarget.iOS)
            {
                return;
            }

            var settings = MobileRuntimePermissionsProjectSettings.instance;
            foreach (var entry in settings.Permissions.Where(item => item.includeInBuild))
            {
                if (!PermissionCatalog.TryGet(entry.permission, out var definition))
                {
                    continue;
                }

                if (!definition.SupportsIos || definition.IosUsageKeys.Length == 0)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(entry.iosUsageDescription))
                {
                    continue;
                }

                throw new BuildFailedException(
                    $"Permission '{entry.permission}' requires an iOS usage description in Project Settings > Mobile Runtime Permissions.");
            }
        }
    }
}
