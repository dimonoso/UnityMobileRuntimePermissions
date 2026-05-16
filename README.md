# Mobile Runtime Permissions

Unity Package Manager package for runtime/mobile privacy permissions on Android and iOS.

## Install from git

Use the repository root directly:

```text
https://github.com/dimonoso/UnityMobileRuntimePermissions.git
```

## Development project

The Unity development project now lives in:

```text
DevProject/
```

The package itself lives at the repository root, so Unity Package Manager can consume it without `?path=...`.

## Package contents

- `Runtime/`
- `Editor/`
- `Plugins/`
- `Samples~/`
- `Documentation~/`
- `package.json`

## Current state

The package already includes:
- runtime API scaffold
- common + platform-specific permission enum
- Project Settings integration
- Android and iOS native bridge scaffolding
- build-time Android/iOS configuration hooks

Some permission flows still need runtime verification and device testing.
