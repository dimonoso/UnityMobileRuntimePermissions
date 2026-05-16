namespace MobileRuntimePermissions
{
    public readonly struct PermissionInfo
    {
        public PermissionInfo(
            Permission permission,
            bool canRequest,
            bool canOpenAppSettings,
            PermissionStatus status,
            string nativeIdentifier,
            string nativeDetails)
        {
            Permission = permission;
            CanRequest = canRequest;
            CanOpenAppSettings = canOpenAppSettings;
            Status = status;
            NativeIdentifier = nativeIdentifier;
            NativeDetails = nativeDetails;
        }

        public Permission Permission { get; }

        public bool IsSupported => Status != PermissionStatus.Unsupported;

        public bool CanRequest { get; }

        public bool CanOpenAppSettings { get; }

        public PermissionStatus Status { get; }

        public string NativeIdentifier { get; }

        public string NativeDetails { get; }
    }
}
