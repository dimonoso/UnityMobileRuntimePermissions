#if UNITY_IOS
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MobileRuntimePermissions.Internal;
using MobileRuntimePermissions.Editor.Settings;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace MobileRuntimePermissions.Editor.Build
{
    internal static class iOSPostBuildProcessor
    {
        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var root = plist.root;
            var settings = MobileRuntimePermissionsProjectSettings.instance;
            var localizedFiles = new Dictionary<string, List<string>>();

            foreach (var entry in settings.Permissions.Where(item => item.includeInBuild))
            {
                if (!PermissionCatalog.TryGet(entry.permission, out var definition))
                {
                    continue;
                }

                if (!definition.SupportsIos)
                {
                    continue;
                }

                foreach (var usageKey in definition.IosUsageKeys)
                {
                    if (!string.IsNullOrWhiteSpace(entry.iosUsageDescription))
                    {
                        root.SetString(usageKey, entry.iosUsageDescription);
                    }

                    foreach (var localizedEntry in entry.localizedUsageDescriptions.Where(item => !string.IsNullOrWhiteSpace(item.locale) && !string.IsNullOrWhiteSpace(item.message)))
                    {
                        var line = $"\"{usageKey}\" = \"{Escape(localizedEntry.message)}\";";
                        if (!localizedFiles.TryGetValue(localizedEntry.locale, out var lines))
                        {
                            lines = new List<string>();
                            localizedFiles[localizedEntry.locale] = lines;
                        }

                        lines.Add(line);
                    }
                }
            }

            plist.WriteToFile(plistPath);

            foreach (var pair in localizedFiles)
            {
                var localeDirectory = Path.Combine(pathToBuiltProject, $"{pair.Key}.lproj");
                Directory.CreateDirectory(localeDirectory);
                var stringsPath = Path.Combine(localeDirectory, "InfoPlist.strings");
                File.WriteAllLines(stringsPath, pair.Value.Distinct());
            }
        }

        private static string Escape(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
#endif
