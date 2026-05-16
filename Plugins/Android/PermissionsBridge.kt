package com.dimonoso.mobileruntimepermissions

import android.app.Activity
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.os.Build
import android.provider.Settings
import com.unity3d.player.UnityPlayer

object PermissionsBridge {
    private const val StatusAllow = 1
    private const val StatusDeny = 0
    private const val StatusDenyNeverAsk = -1

    private var callbackGameObject = "MobileRuntimePermissionsAndroidCallback"

    @JvmStatic
    fun initialize(gameObjectName: String) {
        if (gameObjectName.isNotBlank()) {
            callbackGameObject = gameObjectName
        }
    }

    @JvmStatic
    fun getPermissionStatus(permission: String): Int {
        val activity = currentActivity() ?: return StatusDeny
        return getPermissionStatus(activity, permission)
    }

    @JvmStatic
    fun requestPermission(permission: String, requestId: Int) {
        val activity = currentActivity()
        if (activity == null || permission.isBlank()) {
            dispatchToUnity(requestId, StatusDeny)
            return
        }

        val currentStatus = getPermissionStatus(activity, permission)
        if (currentStatus == StatusAllow || currentStatus == StatusDenyNeverAsk) {
            dispatchToUnity(requestId, currentStatus)
            return
        }

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            dispatchToUnity(requestId, StatusAllow)
            return
        }

        PermissionStateStore.markRequested(activity, permission)
        PermissionsRequestFragment.request(activity, permission, requestId)
    }

    @JvmStatic
    fun openAppSettings(): Boolean {
        val activity = currentActivity() ?: return false
        val intent = Intent(
            Settings.ACTION_APPLICATION_DETAILS_SETTINGS,
            Uri.fromParts("package", activity.packageName, null)
        )
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
        activity.startActivity(intent)
        return true
    }

    @JvmStatic
    fun openPermissionSettings(permission: String): Boolean {
        val activity = currentActivity() ?: return false

        if (permission == "android.permission.POST_NOTIFICATIONS" && Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            val intent = Intent(Settings.ACTION_APP_NOTIFICATION_SETTINGS)
                .putExtra(Settings.EXTRA_APP_PACKAGE, activity.packageName)
                .addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            activity.startActivity(intent)
            return true
        }

        return openAppSettings()
    }

    internal fun finishPermissionRequest(requestId: Int, permission: String, grantResult: Int) {
        val activity = currentActivity()
        if (activity == null) {
            dispatchToUnity(requestId, StatusDeny)
            return
        }

        val status = when {
            grantResult == PackageManager.PERMISSION_GRANTED -> StatusAllow
            activity.shouldShowRequestPermissionRationale(permission) -> StatusDeny
            else -> StatusDenyNeverAsk
        }

        dispatchToUnity(requestId, status)
    }

    private fun currentActivity(): Activity? {
        return UnityPlayer.currentActivity
    }

    private fun getPermissionStatus(activity: Activity, permission: String): Int {
        if (permission.isBlank()) {
            return StatusDeny
        }

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.M) {
            return StatusAllow
        }

        return when {
            activity.checkSelfPermission(permission) == PackageManager.PERMISSION_GRANTED -> StatusAllow
            !PermissionStateStore.wasRequested(activity, permission) -> StatusDeny
            activity.shouldShowRequestPermissionRationale(permission) -> StatusDeny
            else -> StatusDenyNeverAsk
        }
    }

    private fun dispatchToUnity(requestId: Int, status: Int) {
        UnityPlayer.UnitySendMessage(callbackGameObject, "OnPermissionResult", "$requestId|$status")
    }
}
