# Unity Mobile Runtime Permissions

A Unity plugin for checking and requesting **runtime permissions** on **Android** and **iOS**.  
It allows you to:
- Get the current permission status **without showing the native popup**.
- Request permissions with callbacks.

---

## Installation

1. Download and import **`UnityMobileRuntimePermissions.unitypackage`** into your Unity project.

### Android Setup

Add the required permissions and activity to your **AndroidManifest.xml**:

<manifest>
  <application>
    <activity android:name="com.dimonoso.unitypermission.UnityPermissionActivity">
      ...
    </activity>
  </application>

  <uses-permission android:name="android.permission.CAMERA" />
  ...
</manifest>

Custom Activity Options

Option 1: Subclassing
If you want to use your own Activity, subclass com.dimonoso.unitypermission.UnityPermissionActivity.
If you override:

void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)

you must call:

super.onRequestPermissionsResult(requestCode, permissions, grantResults);

Option 2: Replacing the base class
Edit the file:

Assets/MobileRuntimePermission/Plugins/Android/UnityPermissionActivity.java

Change:

public class UnityPermissionActivity extends <YourActivityClass> {

In this case, the launching activity will remain com.dimonoso.unitypermission.UnityPermissionActivity.
UnityPlayerActivity vs UnityPlayerGameActivity

If you need UnityPlayerActivity instead of UnityPlayerGameActivity, edit:

import com.unity3d.player.UnityPlayerGameActivity;

to:

import com.unity3d.player.UnityPlayerActivity;

And update:

public class UnityPermissionActivity extends UnityPlayerGameActivity {

to:

public class UnityPermissionActivity extends UnityPlayerActivity {

Usage
Check permission status without prompting

var status = PermissionManager.PermissionStatus(Permission.CAMERA);

if (status == PermissionRequestStatus.Allow) {
    // Allowed
}
else if (status == PermissionRequestStatus.Deny) {
    // Denied
}
else if (status == PermissionRequestStatus.DenyAndNeverAskAgain) {
    // Denied permanently
}

Request a permission

PermissionManager.RequestPermission(
    new PermissionData(
        Permission.CAMERA,
        onAllow,
        onDeny,
        onDenyNeverAskAgain
    )
);

Known Issues

    Not all possible Android permissions are supported.

    On iOS, currently only CAMERA and RECORD_AUDIO are supported,
    since there is no centralized permission system (or it hasn’t been implemented yet).
