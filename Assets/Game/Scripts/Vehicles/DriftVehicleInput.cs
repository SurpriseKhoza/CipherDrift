using UnityEngine;

namespace CipherDrift.Vehicles
{
    [System.Serializable]
    public struct DriftVehicleInput
    {
        [Range(-1f, 1f)] public float steering;
        [Range(0f, 1f)] public float throttle;
        [Range(0f, 1f)] public float brake;
        [Range(0f, 1f)] public float handbrake;

        public static DriftVehicleInput ReadEditorFallback()
        {
            return new DriftVehicleInput
            {
                steering = Input.GetAxisRaw("Horizontal"),
                throttle = Mathf.Clamp01(Input.GetAxisRaw("Vertical")),
                brake = Mathf.Clamp01(-Input.GetAxisRaw("Vertical")),
                handbrake = Input.GetKey(KeyCode.Space) ? 1f : 0f
            };
        }
    }
}
