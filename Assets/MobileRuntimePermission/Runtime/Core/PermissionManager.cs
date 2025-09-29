using UnityEngine.Events;

namespace MobileRuntimePermission.Core
{
    public enum Permission
    {
        ADD_VOICEMAIL,
        BODY_SENSORS,
        CAMERA,
        READ_EXTERNAL_STORAGE,
        READ_PHONE_STATE,
        RECEIVE_WAP_PUSH,
        RECORD_AUDIO,
        USE_SIP,
        WRITE_EXTERNAL_STORAGE
    }

    public enum PermissionRequestStatus
    {
        Allow = 1,
        Deny = 0,
        DenyAndNeverAskAgain = -1,
    }

    public class PermissionData
    {
        public Permission Permission;
        public UnityAction OnAllow;
        public UnityAction OnDeny;
        public UnityAction OnDenyAndNeverAskAgain;

        public PermissionData(Permission permission, UnityAction onAllow = null, UnityAction onDeny = null, UnityAction onDenyNeverAskAgain = null)
        {
            Permission = permission;
            OnAllow = onAllow;
            OnDeny = onDeny;
            OnDenyAndNeverAskAgain = onDenyNeverAskAgain;
        }
    }
}
