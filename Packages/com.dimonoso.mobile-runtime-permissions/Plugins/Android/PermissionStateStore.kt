package com.dimonoso.mobileruntimepermissions

import android.content.Context

internal object PermissionStateStore {
    private const val PreferencesName = "mobile_runtime_permissions"
    private const val RequestedPrefix = "requested:"

    fun markRequested(context: Context, permission: String) {
        context.getSharedPreferences(PreferencesName, Context.MODE_PRIVATE)
            .edit()
            .putBoolean(RequestedPrefix + permission, true)
            .apply()
    }

    fun wasRequested(context: Context, permission: String): Boolean {
        return context.getSharedPreferences(PreferencesName, Context.MODE_PRIVATE)
            .getBoolean(RequestedPrefix + permission, false)
    }
}
