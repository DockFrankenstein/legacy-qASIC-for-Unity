using UnityEngine;

namespace qASIC.Input.Devices
{
    public static class GamepadUtility
    {
        public static float CalculateDeadZone(float value, float min, float max) =>
            Mathf.Sign(value) * Mathf.Clamp01((Mathf.Abs(value) - min) / (max - min));
    }
}