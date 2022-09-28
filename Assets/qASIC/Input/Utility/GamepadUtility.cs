using UnityEngine;

namespace qASIC.InputManagement.Devices
{
    public static class GamepadUtility
    {
        public static float CalculateDeadZone(float value, float min, float max) =>
            Mathf.Clamp01(Mathf.Sign(value) * ((Mathf.Abs(value) - min) / (max - min)));
    }
}