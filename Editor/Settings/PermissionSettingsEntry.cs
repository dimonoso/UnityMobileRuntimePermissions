using System;
using System.Collections.Generic;

namespace MobileRuntimePermissions.Editor.Settings
{
    [Serializable]
    internal sealed class PermissionSettingsEntry
    {
        public Permission permission;
        public bool includeInBuild;
        public string iosUsageDescription = string.Empty;
        public bool iosLocalizationFoldout;
        public List<LocalizedUsageDescription> localizedUsageDescriptions = new();
    }
}
