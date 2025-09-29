package com.dimonoso.unitypermission;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerGameActivity;

import android.content.Context;
import android.content.pm.PackageManager;

public class UnityPermissionActivity extends UnityPlayerGameActivity {
    public void requestPermission(String permissionStr, int requestCode)
    {
        if ((getPermissionState(permissionStr)) == 0) {
            UnityPlayer.currentActivity.requestPermissions(new String[] { permissionStr }, requestCode);
        }
    }

    public int getPermissionState(String permissionStr)
    {
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        if (context.checkCallingOrSelfPermission(permissionStr) == PackageManager.PERMISSION_GRANTED) {
            return 1;
        }

        boolean showRationale = shouldShowRequestPermissionRationale( permissionStr );

        return showRationale ? 0 : -1;
    }

    public static PermissionCallback permissionCallback;
    public static void initPermissionCallback(PermissionCallback callback){
        permissionCallback = callback;
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        if (permissionCallback != null) {
            int permissionState = 0; // 0 - Denied or not asked; 1 - Granted; -1 - DeniedNeverAsk

            for (int i =0; i < grantResults.length; i++) {
                String permission = permissions[i];
                if (grantResults[i] == PackageManager.PERMISSION_DENIED) {
                    boolean showRationale = shouldShowRequestPermissionRationale( permission );

                    permissionState = showRationale ? 0 : -1;
                    break;
                }
                if (grantResults[i] == PackageManager.PERMISSION_GRANTED) {
                    permissionState = 1;
                }
            }

            permissionCallback.OnRequestPermissionsResult(requestCode, permissionState);
        }

        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
