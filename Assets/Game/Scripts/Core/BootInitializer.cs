using UnityEngine;

namespace CipherDrift.Core
{
    public sealed class BootInitializer : MonoBehaviour
    {
        private void Awake()
        {
            QualityProfile.ApplyStartupProfile();
        }
    }
}
