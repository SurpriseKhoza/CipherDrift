using UnityEngine;

namespace CipherDrift.Vehicles
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class DriftVehicleController : MonoBehaviour
    {
        [Header("References")]
        public DriftWheel[] wheels = new DriftWheel[4];
        public LayerMask groundMask = ~0;

        [Header("Steering and drivetrain prototype")]
        [Range(10f, 55f)] public float maxSteerAngle = 35f;
        [Min(0f)] public float engineForce = 8500f;
        [Min(0f)] public float serviceBrakeForce = 11000f;
        [Min(0f)] public float handbrakeForce = 15000f;

        [Header("Chassis")]
        public Vector3 centerOfMassOffset = new Vector3(0f, -0.35f, 0.05f);
        [Min(0f)] public float aerodynamicDrag = 0.32f;
        [Min(0f)] public float downforce = 25f;

        [Header("Tire model")]
        [Min(0.1f)] public float peakSlipAngle = 9f;
        [Min(0.1f)] public float peakSlipRatio = 0.12f;
        [Min(0.1f)] public float frictionCoefficient = 1.15f;

        private Rigidbody body;
        private DriftVehicleInput currentInput;

        public float SpeedKph => body == null ? 0f : body.linearVelocity.magnitude * 3.6f;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.mass = Mathf.Max(body.mass, 900f);
            body.centerOfMass += centerOfMassOffset;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void SetInput(DriftVehicleInput input)
        {
            currentInput = input;
        }

        private void Update()
        {
            // Editor-only fallback. Replace with Input System actions and mobile UI.
            if (Application.isEditor)
                currentInput = DriftVehicleInput.ReadEditorFallback();
        }

        private void FixedUpdate()
        {
            if (wheels == null || wheels.Length == 0)
                return;

            ApplyAerodynamics();

            foreach (DriftWheel wheel in wheels)
            {
                if (wheel == null) continue;

                if (wheel.steering)
                {
                    float steer = currentInput.steering * maxSteerAngle;
                    wheel.transform.localRotation = Quaternion.Euler(0f, steer, 0f);
                }

                wheel.SampleGround(groundMask);
                if (!wheel.grounded) continue;

                ApplySuspension(wheel);
                ApplyTireForces(wheel);
            }
        }

        private void ApplySuspension(DriftWheel wheel)
        {
            Vector3 pointVelocity = body.GetPointVelocity(wheel.hit.point);
            float suspensionLength = wheel.hit.distance - wheel.radius;
            float compression = wheel.suspensionRestLength - suspensionLength;
            float springVelocity = Vector3.Dot(pointVelocity, wheel.transform.up);

            float springForce = compression * wheel.springStrength;
            float damperForce = springVelocity * wheel.damperStrength;
            float force = Mathf.Max(0f, springForce - damperForce);

            body.AddForceAtPosition(
                wheel.transform.up * force,
                wheel.hit.point,
                ForceMode.Force);
        }

        private void ApplyTireForces(DriftWheel wheel)
        {
            Vector3 velocity = body.GetPointVelocity(wheel.hit.point);
            Vector3 forward = Vector3.ProjectOnPlane(wheel.transform.forward, wheel.hit.normal).normalized;
            Vector3 right = Vector3.ProjectOnPlane(wheel.transform.right, wheel.hit.normal).normalized;

            if (forward.sqrMagnitude < 0.01f || right.sqrMagnitude < 0.01f)
                return;

            float forwardSpeed = Vector3.Dot(velocity, forward);
            float lateralSpeed = Vector3.Dot(velocity, right);

            float slipAngle = Mathf.Atan2(lateralSpeed, Mathf.Abs(forwardSpeed) + 0.5f) * Mathf.Rad2Deg;
            wheel.slipAngleDegrees = slipAngle;

            float normalLoad = Mathf.Max(0f, Vector3.Dot(
                body.GetPointVelocity(wheel.hit.point) * 0f + Physics.gravity * -1f,
                wheel.hit.normal) * body.mass / Mathf.Max(1, wheels.Length));

            float grip = frictionCoefficient * Mathf.Max(wheel.longitudinalGrip, wheel.lateralGrip);
            float maxTireForce = Mathf.Max(1500f, normalLoad * grip);

            float lateralDemand = Mathf.Clamp(-lateralSpeed * 950f, -maxTireForce, maxTireForce);
            float longitudinalDemand = 0f;

            if (wheel.driven)
                longitudinalDemand += currentInput.throttle * engineForce * 0.5f;

            if (wheel.handbrakeWheel)
            {
                longitudinalDemand -= currentInput.handbrake * handbrakeForce * Mathf.Sign(forwardSpeed);
                if (currentInput.handbrake > 0.05f)
                    lateralDemand *= Mathf.Lerp(1f, 0.48f, currentInput.handbrake);
            }

            longitudinalDemand -= currentInput.brake * serviceBrakeForce * Mathf.Sign(forwardSpeed);
            longitudinalDemand -= forwardSpeed * wheel.rollingResistance;

            // Friction-circle clamp: combined longitudinal and lateral force cannot
            // exceed the current tire-force budget.
            float forceMagnitude = Mathf.Sqrt(
                longitudinalDemand * longitudinalDemand +
                lateralDemand * lateralDemand);

            if (forceMagnitude > maxTireForce && forceMagnitude > 0.001f)
            {
                float scale = maxTireForce / forceMagnitude;
                longitudinalDemand *= scale;
                lateralDemand *= scale;
            }

            Vector3 tireForce = forward * longitudinalDemand + right * lateralDemand;
            body.AddForceAtPosition(tireForce, wheel.hit.point, ForceMode.Force);
        }

        private void ApplyAerodynamics()
        {
            Vector3 velocity = body.linearVelocity;
            body.AddForce(-velocity * aerodynamicDrag, ForceMode.Force);
            body.AddForce(-transform.up * (velocity.sqrMagnitude * downforce), ForceMode.Force);
        }
    }
}
