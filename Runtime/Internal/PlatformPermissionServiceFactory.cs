namespace MobileRuntimePermissions.Internal
{
    internal static class PlatformPermissionServiceFactory
    {
        public static IPlatformPermissionService Create()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidPermissionService();
#elif UNITY_IOS && !UNITY_EDITOR
            return new iOSPermissionService();
#else
            return new UnsupportedPermissionService();
#endif
        }
    }
}
