using System;
using System.Collections.Generic;
using System.Linq;
using MobileRuntimePermissions.Internal;
using UnityEditor;
using UnityEngine;

namespace MobileRuntimePermissions.Editor.Settings
{
    internal static class MobileRuntimePermissionsSettingsProvider
    {
        private const int AndroidTabIndex = 0;

        private const string SharedPermissionHelp =
            "Permissions marked with * are shared settings between Android and iOS. Enabling or disabling them on one platform applies to the other platform as well.";

        private const string CustomLocaleLabel = "Custom...";

        private static readonly Dictionary<string, string> SystemLanguageLocaleCodes = new()
        {
            ["Afrikaans"] = "af",
            ["Arabic"] = "ar",
            ["Basque"] = "eu",
            ["Belarusian"] = "be",
            ["Bulgarian"] = "bg",
            ["Catalan"] = "ca",
            ["Chinese"] = "zh",
            ["ChineseSimplified"] = "zh-Hans",
            ["ChineseTraditional"] = "zh-Hant",
            ["Croatian"] = "hr",
            ["Czech"] = "cs",
            ["Danish"] = "da",
            ["Dutch"] = "nl",
            ["English"] = "en",
            ["Estonian"] = "et",
            ["Faroese"] = "fo",
            ["Finnish"] = "fi",
            ["French"] = "fr",
            ["German"] = "de",
            ["Greek"] = "el",
            ["Hebrew"] = "he",
            ["Hindi"] = "hi",
            ["Hungarian"] = "hu",
            ["Icelandic"] = "is",
            ["Indonesian"] = "id",
            ["Italian"] = "it",
            ["Japanese"] = "ja",
            ["Korean"] = "ko",
            ["Latvian"] = "lv",
            ["Lithuanian"] = "lt",
            ["Norwegian"] = "nb",
            ["Polish"] = "pl",
            ["Portuguese"] = "pt",
            ["Romanian"] = "ro",
            ["Russian"] = "ru",
            ["SerboCroatian"] = "sr-Latn",
            ["Slovak"] = "sk",
            ["Slovenian"] = "sl",
            ["Spanish"] = "es",
            ["Swedish"] = "sv",
            ["Thai"] = "th",
            ["Turkish"] = "tr",
            ["Ukrainian"] = "uk",
            ["Vietnamese"] = "vi",
        };

        private static readonly string[] PlatformTabs = { "Android", "iOS" };
        private static int selectedPlatformTab;
        private static Vector2 androidScrollPosition;
        private static Vector2 iosScrollPosition;
        private static string[] languagePopupLabels;
        private static string[] languagePopupLocaleCodes;

        private enum PlatformTab
        {
            Android,
            Ios,
        }

        private readonly struct LanguageOption
        {
            public LanguageOption(string label, string localeCode)
            {
                Label = label;
                LocaleCode = localeCode;
            }

            public string Label { get; }

            public string LocaleCode { get; }
        }

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

            selectedPlatformTab = GUILayout.Toolbar(selectedPlatformTab, PlatformTabs);
            EditorGUILayout.HelpBox(SharedPermissionHelp, MessageType.Info);

            var platform = selectedPlatformTab == AndroidTabIndex ? PlatformTab.Android : PlatformTab.Ios;
            var scrollPosition = platform == PlatformTab.Android ? androidScrollPosition : iosScrollPosition;

            using var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition);
            if (platform == PlatformTab.Android)
            {
                androidScrollPosition = scrollView.scrollPosition;
            }
            else
            {
                iosScrollPosition = scrollView.scrollPosition;
            }

            foreach (var entry in settings.Permissions)
            {
                if (!PermissionCatalog.TryGet(entry.permission, out var definition))
                {
                    continue;
                }

                if (!SupportsPlatform(definition, platform))
                {
                    continue;
                }

                DrawPermissionEntry(settings, entry, definition, platform);
            }
        }

        private static void DrawPermissionEntry(
            MobileRuntimePermissionsProjectSettings settings,
            PermissionSettingsEntry entry,
            PermissionDefinition definition,
            PlatformTab platform)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            entry.includeInBuild = EditorGUILayout.ToggleLeft(
                BuildToggleLabel(entry, definition),
                entry.includeInBuild);

            using (new EditorGUI.DisabledScope(true))
            {
                DrawPlatformDetails(definition, platform);
            }

            if (platform == PlatformTab.Ios && definition.IosUsageKeys.Length > 0)
            {
                entry.iosUsageDescription = EditorGUILayout.TextField("iOS Default Text", entry.iosUsageDescription);
                entry.iosLocalizationFoldout = EditorGUILayout.Foldout(entry.iosLocalizationFoldout, "iOS Localizations", true);
                if (entry.iosLocalizationFoldout)
                {
                    for (var index = 0; index < entry.localizedUsageDescriptions.Count; index++)
                    {
                        var localizedEntry = entry.localizedUsageDescriptions[index];
                        EditorGUILayout.BeginHorizontal();
                        DrawLocaleField(localizedEntry);
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

        private static void DrawLocaleField(LocalizedUsageDescription localizedEntry)
        {
            EnsureLanguageOptions();

            var currentLocale = localizedEntry.locale ?? string.Empty;
            var selectedIndex = FindLocaleIndex(currentLocale);
            var customIndex = languagePopupLabels.Length - 1;
            if (selectedIndex < 0)
            {
                selectedIndex = customIndex;
            }

            var newIndex = EditorGUILayout.Popup(
                selectedIndex,
                languagePopupLabels,
                GUILayout.MinWidth(170f),
                GUILayout.MaxWidth(220f));

            if (newIndex != customIndex)
            {
                localizedEntry.locale = languagePopupLocaleCodes[newIndex];
                return;
            }

            if (selectedIndex != customIndex)
            {
                currentLocale = string.Empty;
            }

            localizedEntry.locale = EditorGUILayout.TextField(
                currentLocale,
                GUILayout.MinWidth(64f),
                GUILayout.MaxWidth(96f));
        }

        private static bool SupportsPlatform(PermissionDefinition definition, PlatformTab platform)
        {
            return platform == PlatformTab.Android
                ? definition.SupportsAndroid
                : definition.SupportsIos;
        }

        private static GUIContent BuildToggleLabel(PermissionSettingsEntry entry, PermissionDefinition definition)
        {
            var sharedSuffix = IsShared(definition) ? "*" : string.Empty;
            var label = $"{definition.DisplayName}{sharedSuffix} ({entry.permission})";
            var tooltip = IsShared(definition) ? SharedPermissionHelp : string.Empty;

            return new GUIContent(label, tooltip);
        }

        private static void DrawPlatformDetails(PermissionDefinition definition, PlatformTab platform)
        {
            if (platform == PlatformTab.Android)
            {
                EditorGUILayout.TextField("Android Names", BuildJoinedLabel(definition.AndroidPermissionNames));
                EditorGUILayout.IntField("Android Runtime Min API", definition.AndroidRuntimeMinApi);
                return;
            }

            EditorGUILayout.TextField("iOS Keys", BuildJoinedLabel(definition.IosUsageKeys));
        }

        private static string BuildJoinedLabel(string[] values)
        {
            return string.Join(", ", values.Where(value => !string.IsNullOrEmpty(value)));
        }

        private static bool IsShared(PermissionDefinition definition)
        {
            return definition.SupportsAndroid && definition.SupportsIos;
        }

        private static void EnsureLanguageOptions()
        {
            if (languagePopupLabels != null && languagePopupLocaleCodes != null)
            {
                return;
            }

            var options = new List<LanguageOption>();
            foreach (SystemLanguage language in Enum.GetValues(typeof(SystemLanguage)))
            {
                var languageName = language.ToString();
                if (languageName == "Unknown" ||
                    !SystemLanguageLocaleCodes.TryGetValue(languageName, out var localeCode))
                {
                    continue;
                }

                options.Add(
                    new LanguageOption(
                        $"{ObjectNames.NicifyVariableName(languageName)} ({localeCode})",
                        localeCode));
            }

            options.Sort((left, right) => string.Compare(left.Label, right.Label, StringComparison.Ordinal));

            languagePopupLabels = options.Select(option => option.Label).Append(CustomLocaleLabel).ToArray();
            languagePopupLocaleCodes = options.Select(option => option.LocaleCode).Append(string.Empty).ToArray();
        }

        private static int FindLocaleIndex(string locale)
        {
            if (string.IsNullOrEmpty(locale) || languagePopupLocaleCodes == null)
            {
                return -1;
            }

            for (var index = 0; index < languagePopupLocaleCodes.Length - 1; index++)
            {
                if (string.Equals(languagePopupLocaleCodes[index], locale, StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
