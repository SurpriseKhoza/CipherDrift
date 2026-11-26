#if UNITY_EDITOR
namespace CipherDrift.Editor.BuildAutomation
{
    public static class BuildEntrypoint
    {
        public static void BuildAndroid()
        {
            DriftAndroidBuild.CreateStarterScenes();
            DriftAndroidBuild.BuildAndroidInternal();
        }
    }
}
#endif
