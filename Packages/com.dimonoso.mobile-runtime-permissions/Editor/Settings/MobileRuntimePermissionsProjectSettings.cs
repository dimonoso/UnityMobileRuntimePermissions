using System;
using System.Collections.Generic;
using System.Linq;
using MobileRuntimePermissions.Internal;
using UnityEditor;
using UnityEngine;

namespace MobileRuntimePermissions.Editor.Settings
{
    [FilePath("ProjectSettings/MobileRuntimePermissions.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class MobileRuntimePermissionsProjectSettings : ScriptableSingleton<MobileRuntimePermissionsProjectSettings>
    {
        [SerializeField]
        private List<PermissionSettingsEntry> permissions = new();

        public IReadOnlyList<PermissionSettingsEntry> Permissions
        {
            get
            {
                EnsureInitialized();
                return permissions;
            }
        }

        public void SaveSettings()
        {
            Save(true);
        }

        public PermissionSettingsEntry GetEntry(Permission permission)
        {
            EnsureInitialized();
            return permissions.First(entry => entry.permission == permission);
        }

        private void OnEnable()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            var knownPermissions = new HashSet<Permission>(Enum.GetValues(typeof(Permission)).Cast<Permission>());
            permissions.RemoveAll(entry => !knownPermissions.Contains(entry.permission));

            foreach (var permission in Enum.GetValues(typeof(Permission)).Cast<Permission>())
            {
                if (permissions.Any(entry => entry.permission == permission))
                {
                    continue;
                }

                permissions.Add(
                    new PermissionSettingsEntry
                    {
                        permission = permission,
                    });
            }

            permissions.Sort((left, right) => left.permission.CompareTo(right.permission));

            foreach (var entry in permissions)
            {
                if (!PermissionCatalog.TryGet(entry.permission, out var definition))
                {
                    continue;
                }

                if (definition.IosUsageKeys.Length == 0)
                {
                    entry.localizedUsageDescriptions.Clear();
                }
            }
        }
    }
}
