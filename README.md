# CipherDrift

Original Android drift-racing game starter project. This is a Unity source scaffold, not a finished game or compiled APK.

## Project baseline

- Unity Editor: 6000.3.0f1 (Unity 6.3 LTS baseline; verify the exact installed patch before opening)
- URP: 17.3.0
- Input System: 1.14.2
- Android: IL2CPP, ARMv7 + ARM64
- Rendering: URP, scalable quality tiers
- Distribution target: direct APK + main OBB expansion file
- Gameplay: offline career first, authoritative online drift rooms later

## First launch

1. Install the matching Unity 6.3 Editor with Android Build Support, Android SDK & NDK tools, and OpenJDK.
2. Open this folder in Unity Hub.
3. Allow Package Manager to resolve packages.
4. From the Unity menu, run **CipherDrift > Create Starter Scenes**.
5. Create a test vehicle GameObject with a Rigidbody and four child wheel anchors.
6. Add `DriftVehicleController` to the vehicle root and assign the four `DriftWheel` components.
7. Add a ground collider and a simple test track.
8. Press Play and use the keyboard input fallback (WASD / Space) in the editor.

## Important prototype limitations

The included vehicle code is a starting point for a custom raycast suspension and simplified combined-slip tire model. It is not yet a validated professional tire model. Before competitive play, calibrate it against reference handling data, add a proper tire load curve, drivetrain/clutch simulation, differential behavior, thermal/wear models, fixed-step determinism tests and extensive device profiling.

Online multiplayer is deliberately not enabled in this first scaffold. The future server must authoritatively validate movement, collisions and scoring. Never trust client-submitted drift scores.

## Android / OBB

The editor build script configures IL2CPP and ARMv7 + ARM64 and enables Unity's legacy APK expansion-file setting. Unity's actual OBB output and naming must be verified on the pinned editor version. Keep APK and OBB from the same build together. Do not manually rename or extract Unity-generated OBB files.

OBB is intended here for direct APK distribution. For Google Play distribution, use Play Asset Delivery instead.

## GitHub Actions

- `validate.yml`: checks source layout and whitespace.
- `android-build.yml`: GameCI Unity build job. Add `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` repository secrets, and verify the runner/editor license terms before use.
- The first Android workflow is a build scaffold. Confirm the artifact paths and generated OBB on the first successful Unity build before publishing.

## Suggested next implementation order

1. Build a greybox car and one small drift pad.
2. Calibrate suspension, tire grip and steering.
3. Add engine, gearbox, clutch and differential.
4. Add drift scoring and offline event progression.
5. Add car customization and garage.
6. Add authoritative online drift rooms and backend.
7. Add production content, quality tiers and OBB asset layout.
