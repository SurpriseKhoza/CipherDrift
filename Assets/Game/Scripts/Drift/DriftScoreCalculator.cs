using UnityEngine;

namespace CipherDrift.Drift
{
    [System.Serializable]
    public sealed class DriftScoreCalculator
    {
        [Min(0f)] public float angleWeight = 1.0f;
        [Min(0f)] public float speedWeight = 0.35f;
        [Min(0f)] public float lineWeight = 0.5f;
        [Min(0f)] public float comboDecayPerSecond = 0.25f;

        private float combo = 1f;

        public float Combo => combo;

        public int Evaluate(float slipAngleDegrees, float speedKph, float lineQuality, float deltaTime)
        {
            float angle = Mathf.Clamp01(Mathf.Abs(slipAngleDegrees) / 55f);
            float speed = Mathf.Clamp01(speedKph / 120f);
            float line = Mathf.Clamp01(lineQuality);

            bool drifting = Mathf.Abs(slipAngleDegrees) >= 8f && speedKph >= 15f;
            if (!drifting)
            {
                combo = Mathf.MoveTowards(combo, 1f, comboDecayPerSecond * deltaTime);
                return 0;
            }

            combo = Mathf.Min(5f, combo + deltaTime * 0.18f);
            float quality = angle * angleWeight + speed * speedWeight + line * lineWeight;
            return Mathf.Max(0, Mathf.RoundToInt(quality * combo * deltaTime * 100f));
        }

        public void ResetCombo()
        {
            combo = 1f;
        }
    }
}
