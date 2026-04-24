# Permission Matrix

## Common

- `Camera`
- `Microphone`
- `Photos`
- `Location`
- `Notifications`
- `Bluetooth`
- `LocalNetwork`

## Android-specific

- `_AndroidLocationCoarse`
- `_AndroidLocationPrecise`
- `_AndroidLocationBackground`
- `_AndroidBluetoothScan`
- `_AndroidBluetoothConnect`
- `_AndroidBluetoothAdvertise`
- `_AndroidReadMediaImages`
- `_AndroidReadMediaVideo`
- `_AndroidReadMediaAudio`
- `_AndroidReadMediaVisualUserSelected`
- `_AndroidLocalNetwork`

## iOS-specific

- `_iOSPhotoLibraryAddOnly`
- `_iOSLocationWhenInUse`
- `_iOSLocationAlways`

## Notes

- Non-popup permissions such as network state are intentionally excluded.
- `Photos` on Android is aimed at direct library access, not the system photo picker flow.
- `LocalNetwork` is included as a runtime/privacy target, but platform support is still evolving and should be validated on target OS versions.
