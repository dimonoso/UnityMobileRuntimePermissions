using System;

namespace MobileRuntimePermissions.Editor.Settings
{
    [Serializable]
    internal sealed class LocalizedUsageDescription
    {
        public string locale = "en";
        public string message = string.Empty;
    }
}
