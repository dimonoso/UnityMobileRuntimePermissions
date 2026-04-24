#if UNITY_ANDROID
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MobileRuntimePermissions.Editor.Settings;
using MobileRuntimePermissions.Internal;
using UnityEditor.Android;

namespace MobileRuntimePermissions.Editor.Build
{
    internal sealed class AndroidPostGenerateGradleProject : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var manifestPath = FindPrimaryManifest(path);
            if (string.IsNullOrEmpty(manifestPath) || !File.Exists(manifestPath))
            {
                return;
            }

            var settings = MobileRuntimePermissionsProjectSettings.instance;
            var permissions = new HashSet<string>();

            foreach (var entry in settings.Permissions.Where(item => item.includeInBuild))
            {
                if (!PermissionCatalog.TryGet(entry.permission, out var definition) || !definition.SupportsAndroid)
                {
                    continue;
                }

                foreach (var permissionName in definition.AndroidPermissionNames.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    permissions.Add(permissionName);
                }

                if (entry.permission == Permission.Photos)
                {
                    permissions.Add("android.permission.READ_EXTERNAL_STORAGE");
                }
            }

            var document = new XmlDocument();
            document.Load(manifestPath);

            var manifestNode = document.SelectSingleNode("/manifest");
            if (manifestNode == null)
            {
                return;
            }

            var androidNamespace = manifestNode.GetNamespaceOfPrefix("android");
            if (string.IsNullOrEmpty(androidNamespace))
            {
                androidNamespace = "http://schemas.android.com/apk/res/android";
            }

            foreach (var permission in permissions)
            {
                var exists = document.SelectSingleNode($"/manifest/uses-permission[@android:name='{permission}']", CreateNamespaceManager(document, androidNamespace));
                if (exists != null)
                {
                    continue;
                }

                var permissionNode = document.CreateElement("uses-permission");
                var nameAttribute = document.CreateAttribute("android", "name", androidNamespace);
                nameAttribute.Value = permission;
                permissionNode.Attributes.Append(nameAttribute);
                manifestNode.AppendChild(permissionNode);
            }

            document.Save(manifestPath);
        }

        private static XmlNamespaceManager CreateNamespaceManager(XmlDocument document, string androidNamespace)
        {
            var namespaceManager = new XmlNamespaceManager(document.NameTable);
            namespaceManager.AddNamespace("android", androidNamespace);
            return namespaceManager;
        }

        private static string FindPrimaryManifest(string path)
        {
            var manifests = Directory.GetFiles(path, "AndroidManifest.xml", SearchOption.AllDirectories);
            return manifests
                .OrderByDescending(item => item.Contains($"{Path.DirectorySeparatorChar}launcher{Path.DirectorySeparatorChar}"))
                .ThenBy(item => item.Length)
                .FirstOrDefault();
        }
    }
}
#endif
