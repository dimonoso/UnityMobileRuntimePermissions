# Unity Mobile Runtime Permissions

Unity repository for an embedded UPM package that manages runtime/mobile privacy permissions on Android and iOS.

Package path:

```text
/Packages/com.dimonoso.mobile-runtime-permissions
```

Install from git:

```text
https://github.com/dimonoso/UnityMobileRuntimePermissions.git?path=/Packages/com.dimonoso.mobile-runtime-permissions
```

Current implementation includes:
- embedded UPM package scaffold
- common + platform-specific permission enum
- runtime permission manager API
- Project Settings integration
- Android/iOS build-time configuration hooks
- Android Java/Kotlin source plugin scaffold
- iOS native bridge scaffold

The package is still under active development, and some platform-specific flows are currently scaffolded rather than fully finalized.
