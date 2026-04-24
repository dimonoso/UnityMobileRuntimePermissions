package com.dimonoso.mobileruntimepermissions;

public final class PermissionsUnityCallback {
    private PermissionsUnityCallback() {
    }

    public static String buildPayload(int requestId, int status) {
        return requestId + "|" + status;
    }
}
