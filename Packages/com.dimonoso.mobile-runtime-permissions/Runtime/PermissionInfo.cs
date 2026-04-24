namespace MobileRuntimePermissions
{
    public readonly struct PermissionInfo
    {
        public PermissionInfo(
            Permission permission,
            bool isSupported,
            bool canRequest,
            bool canOpenAppSettings,
            PermissionStatus status,
            string nativeIdentifier,
            string nativeDetails)
        {
            Permission = permission;
            IsSupported = isSupported;
            CanRequest = canRequest;
            CanOpenAppSettings = canOpenAppSettings;
            Status = status;
            NativeIdentifier = nativeIdentifier;
            NativeDetails = nativeDetails;
        }

        public Permission Permission { get; }

        public bool IsSupported { get; }

        public bool CanRequest { get; }

        public bool CanOpenAppSettings { get; }

        public PermissionStatus Status { get; }

        public string NativeIdentifier { get; }

        public string NativeDetails { get; }
    }
}
