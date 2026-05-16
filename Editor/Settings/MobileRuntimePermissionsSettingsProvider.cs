using System.Linq;
using MobileRuntimePermissions.Internal;
using UnityEditor;
using UnityEngine;

namespace MobileRuntimePermissions.Editor.Settings
{
    internal static class MobileRuntimePermissionsSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Mobile Runtime Permissions", SettingsScope.Project)
            {
                label = "Mobile Runtime Permissions",
                guiHandler = DrawGui,
                keywords = new[] { "permissions", "android", "ios", "runtime", "privacy" },
            };
        }

        private static void DrawGui(string searchContext)
        {
            var settings = MobileRuntimePermissionsProjectSettings.instance;

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Included Permissions", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Only runtime/privacy permissions should be enabled here. Android manifest changes are applied to generated build output, not the project manifest.",
                MessageType.Info);

            using var scrollView = new EditorGUILayout.ScrollViewScope(Vector2.zero);
            foreach (var entry in settings.Permissions)
            {
                if (!PermissionCatalog.TryGet(entry.permission, out var definition))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical("box");
                EditorGUI.BeginChangeCheck();

                entry.includeInBuild = EditorGUILayout.ToggleLeft(
                    $"{definition.DisplayName} ({entry.permission})",
                    entry.includeInBuild);

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField("Platforms", BuildPlatformLabel(definition));
                    EditorGUILayout.TextField("Android Names", string.Join(", ", definition.AndroidPermissionNames.Where(value => !string.IsNullOrEmpty(value))));
                    EditorGUILayout.TextField("iOS Keys", string.Join(", ", definition.IosUsageKeys.Where(value => !string.IsNullOrEmpty(value))));
                }

                if (definition.SupportsIos && definition.IosUsageKeys.Length > 0)
                {
                    entry.iosUsageDescription = EditorGUILayout.TextField("iOS Default Text", entry.iosUsageDescription);
                    entry.iosLocalizationFoldout = EditorGUILayout.Foldout(entry.iosLocalizationFoldout, "iOS Localizations", true);
                    if (entry.iosLocalizationFoldout)
                    {
                        for (var index = 0; index < entry.localizedUsageDescriptions.Count; index++)
                        {
                            var localizedEntry = entry.localizedUsageDescriptions[index];
                            EditorGUILayout.BeginHorizontal();
                            localizedEntry.locale = EditorGUILayout.TextField(localizedEntry.locale, GUILayout.MaxWidth(70f));
                            localizedEntry.message = EditorGUILayout.TextField(localizedEntry.message);
                            if (GUILayout.Button("-", GUILayout.Width(24f)))
                            {
                                entry.localizedUsageDescriptions.RemoveAt(index);
                                GUIUtility.ExitGUI();
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        if (GUILayout.Button("Add Localization"))
                        {
                            entry.localizedUsageDescriptions.Add(new LocalizedUsageDescription());
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    settings.SaveSettings();
                }

                EditorGUILayout.EndVertical();
            }
        }

        private static string BuildPlatformLabel(PermissionDefinition definition)
        {
            if (definition.SupportsAndroid && definition.SupportsIos)
            {
                return "Android, iOS";
            }

            if (definition.SupportsAndroid)
            {
                return "Android";
            }

            if (definition.SupportsIos)
            {
                return "iOS";
            }

            return "None";
        }
    }
}
