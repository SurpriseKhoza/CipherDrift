using UnityEngine;

namespace CipherDrift.Vehicles
{
    [DisallowMultipleComponent]
    public sealed class DriftWheel : MonoBehaviour
    {
        [Header("Wheel geometry")]
        [Min(0.05f)] public float radius = 0.34f;
        [Min(0.05f)] public float suspensionRestLength = 0.28f;
        [Min(0.01f)] public float suspensionTravel = 0.16f;

        [Header("Suspension")]
        [Min(0f)] public float springStrength = 32000f;
        [Min(0f)] public float damperStrength = 4200f;

        [Header("Tire")]
        [Min(0.1f)] public float longitudinalGrip = 1.15f;
        [Min(0.1f)] public float lateralGrip = 1.35f;
        [Min(0f)] public float rollingResistance = 18f;
        public bool steering;
        public bool driven;
        public bool handbrakeWheel;

        [HideInInspector] public bool grounded;
        [HideInInspector] public RaycastHit hit;
        [HideInInspector] public float compression;
        [HideInInspector] public float slipAngleDegrees;
        [HideInInspector] public float longitudinalSlip;

        public Vector3 WorldForward => transform.forward;
        public Vector3 WorldRight => transform.right;

        public float CastDistance => suspensionRestLength + suspensionTravel + radius;

        public void SampleGround(LayerMask groundMask)
        {
            grounded = Physics.Raycast(
                transform.position,
                -transform.up,
                out hit,
                CastDistance,
                groundMask,
                QueryTriggerInteraction.Ignore);

            if (!grounded)
            {
                compression = 0f;
                slipAngleDegrees = 0f;
                longitudinalSlip = 0f;
                return;
            }

            float suspensionLength = hit.distance - radius;
            compression = Mathf.Clamp(
                (suspensionRestLength - suspensionLength) / Mathf.Max(0.01f, suspensionTravel),
                -1f, 1f);
        }
    }
}
