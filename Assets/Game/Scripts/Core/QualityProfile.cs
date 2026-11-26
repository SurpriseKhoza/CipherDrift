using UnityEngine;

namespace CipherDrift.Core
{
    public enum DeviceQualityTier
    {
        Low,
        Balanced,
        High,
        Ultra
    }

    public static class QualityProfile
    {
        public static DeviceQualityTier DetectTier()
        {
            int memory = SystemInfo.systemMemorySize;
            int gpu = SystemInfo.graphicsMemorySize;
            int cores = SystemInfo.processorCount;

            if (memory <= 3000 || gpu <= 512 || cores <= 4)
                return DeviceQualityTier.Low;

            if (memory >= 8000 && gpu >= 2048 && cores >= 8)
                return DeviceQualityTier.Ultra;

            if (memory >= 6000 && gpu >= 1024)
                return DeviceQualityTier.High;

            return DeviceQualityTier.Balanced;
        }

        public static void ApplyStartupProfile()
        {
            DeviceQualityTier tier = DetectTier();
            int index = tier switch
            {
                DeviceQualityTier.Low => 0,
                DeviceQualityTier.Balanced => 1,
                DeviceQualityTier.High => 2,
                _ => 3
            };

            QualitySettings.SetQualityLevel(
                Mathf.Clamp(index, 0, QualitySettings.names.Length - 1),
                true);

            Application.targetFrameRate = tier == DeviceQualityTier.Low ? 30 : 60;
        }
    }
}
