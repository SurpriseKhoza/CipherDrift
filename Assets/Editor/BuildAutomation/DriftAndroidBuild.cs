#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CipherDrift.Editor.BuildAutomation
{
    public static class DriftAndroidBuild
    {
        private const string PackageName = "com.ciphertun.drift";

        [MenuItem("CipherDrift/Create Starter Scenes")]
        public static void CreateStarterScenes()
        {
            Directory.CreateDirectory("Assets/Scenes");
            CreateScene("Assets/Scenes/Boot.unity", "Boot");
            CreateScene("Assets/Scenes/Garage.unity", "Garage");
            CreateScene("Assets/Scenes/DriftTestTrack.unity", "DriftTestTrack");
            AssetDatabase.Refresh();
            Debug.Log("CipherDrift starter scenes created.");
        }

        private static void CreateScene(string path, string name)
        {
            if (File.Exists(path)) return;

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects,
                NewSceneMode.Single);

            scene.name = name;
            if (name == "Boot")
            {
                GameObject boot = new GameObject("BootInitializer");
                boot.AddComponent<CipherDrift.Core.BootInitializer>();
            }

            EditorSceneManager.SaveScene(scene, path);
        }

        [MenuItem("CipherDrift/Build/Android APK + OBB")]
        public static void BuildAndroid()
        {
            BuildAndroidInternal();
        }

        public static void BuildAndroidInternal()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                EditorUserBuildSettings.SwitchActiveBuildTarget(
                    BuildTargetGroup.Android, BuildTarget.Android);

            Directory.CreateDirectory("Builds/Android");

            PlayerSettings.SetApplicationIdentifier(
                UnityEditor.Build.NamedBuildTarget.Android, PackageName);
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.targetArchitectures =
                AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(
                UnityEditor.Build.NamedBuildTarget.Android,
                ScriptingImplementation.IL2CPP);

            // Legacy expansion-file mode for direct APK + OBB distribution.
            PlayerSettings.Android.useAPKExpansionFiles = true;

            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;

            string[] scenePaths =
            {
                "Assets/Scenes/Boot.unity",
                "Assets/Scenes/Garage.unity",
                "Assets/Scenes/DriftTestTrack.unity"
            };

            foreach (string scenePath in scenePaths)
            {
                if (!File.Exists(scenePath))
                    throw new FileNotFoundException(
                        "Starter scene missing. Run CipherDrift > Create Starter Scenes.", scenePath);
            }

            EditorBuildSettings.scenes = Array.ConvertAll(
                scenePaths, path => new EditorBuildSettingsScene(path, true));

            string apkPath = Path.GetFullPath("Builds/Android/CipherDrift.apk");
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Android build failed: " + report.summary.result);

            Debug.Log("APK build completed: " + apkPath +
                      ". Verify the generated OBB beside the APK before distribution.");
        }
    }
}
#endif
