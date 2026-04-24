# Mobile Runtime Permissions

Embedded Unity package for querying and requesting runtime permissions on Android and iOS.

Current package scope:
- common + platform-specific permission enum
- silent status checks
- request API
- open app settings API
- Project Settings configuration
- build-time permission injection for Android and iOS

This is an in-progress implementation. The public API is already scaffolded, and platform integrations are being filled in incrementally.

## Install from git

Use Unity Package Manager with a git URL and `path`:

```text
https://github.com/dimonoso/UnityMobileRuntimePermissions.git?path=/Packages/com.dimonoso.mobile-runtime-permissions
```

## Runtime API

```csharp
using MobileRuntimePermissions;

var info = await PermissionManager.GetInfoAsync(Permission.Camera);
var status = await PermissionManager.RequestAsync(Permission.Camera);
PermissionManager.OpenAppSettings();
```

## Project Settings

Open `Project Settings > Mobile Runtime Permissions` to:
- choose which permissions are included in builds
- configure iOS usage descriptions
- configure iOS localized messages

## Notes

- Only permissions with a real runtime/privacy flow are included.
- `NetworkState` and other non-popup permissions are intentionally excluded.
- Android is implemented via Java/Kotlin source plugins in `Plugins/Android`, not AAR files.
