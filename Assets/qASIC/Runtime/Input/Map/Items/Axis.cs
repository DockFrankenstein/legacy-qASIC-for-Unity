﻿using System;

namespace qASIC.Input.Map
{
    [Serializable]
    public struct Axis
    {
        public Axis(string positiveGuid, string negativeGuid) : this()
        {
            this.positiveGuid = positiveGuid;
            this.negativeGuid = negativeGuid;
        }

        public Axis(string axisGuid, string positiveGuid, string negativeGuid) : this(positiveGuid, negativeGuid)
        {
            this.axisGuid = axisGuid;
        }

        public string axisGuid;
        public string positiveGuid;
        public string negativeGuid;

        public bool IsUsingAxis() =>
            !string.IsNullOrWhiteSpace(axisGuid);

        public float ReadValue(InputMap map, Func<string, float> func)
        {
            if (IsUsingAxis())
            {
                Input1DAxis axis = map.GetItem<Input1DAxis>(axisGuid);
                return axis?.ReadValue(func) ?? 0f;
            }

            InputBinding positive = map.GetItem<InputBinding>(positiveGuid);
            InputBinding negative = map.GetItem<InputBinding>(negativeGuid);

            return positive?.ReadValue(func) ?? 0f - negative?.ReadValue(func) ?? 0f;
        }

        public InputEventType GetInputEvent(InputMap map, Func<string, InputEventType> func)
        {
            if (IsUsingAxis())
            {
                Input1DAxis axis = map.GetItem<Input1DAxis>(axisGuid);
                return axis?.GetInputEvent(func) ?? InputEventType.None;
            }

            InputBinding positive = map.GetItem<InputBinding>(positiveGuid);
            InputBinding negative = map.GetItem<InputBinding>(negativeGuid);

            return positive?.GetInputEvent(func) ?? InputEventType.None |
                negative?.GetInputEvent(func) ?? InputEventType.None;
        }

        public bool HasErrors(InputMap map)
        {
            if (IsUsingAxis())
                return InputMapUtility.IsGuidBroken<Input1DAxis>(map, axisGuid);

            return InputMapUtility.IsGuidBroken<InputBinding>(map, positiveGuid) ||
                InputMapUtility.IsGuidBroken<InputBinding>(map, negativeGuid);
        }
    }
}