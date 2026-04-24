package com.dimonoso.mobileruntimepermissions

import android.app.Activity
import android.app.Fragment
import android.content.pm.PackageManager

internal class PermissionsRequestFragment : Fragment() {
    companion object {
        private const val TagPrefix = "MRP_Request_"

        @JvmStatic
        fun request(activity: Activity, permission: String, requestId: Int) {
            val fragmentManager = activity.fragmentManager ?: return
            val tag = TagPrefix + requestId

            var fragment = fragmentManager.findFragmentByTag(tag) as? PermissionsRequestFragment
            if (fragment == null) {
                fragment = PermissionsRequestFragment()
                fragmentManager.beginTransaction()
                    .add(fragment, tag)
                    .commitAllowingStateLoss()
                fragmentManager.executePendingTransactions()
            }

            fragment.beginRequest(permission, requestId)
        }
    }

    private var hasStarted = false

    fun beginRequest(permission: String, requestId: Int) {
        if (hasStarted) {
            return
        }

        hasStarted = true
        requestPermissions(arrayOf(permission), requestId)
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<String>,
        grantResults: IntArray
    ) {
        val permission = if (permissions.isNotEmpty()) permissions[0] else ""
        val grantResult = if (grantResults.isNotEmpty()) {
            grantResults[0]
        } else {
            PackageManager.PERMISSION_DENIED
        }

        PermissionsBridge.finishPermissionRequest(requestCode, permission, grantResult)

        fragmentManager?.beginTransaction()
            ?.remove(this)
            ?.commitAllowingStateLoss()
    }
}
