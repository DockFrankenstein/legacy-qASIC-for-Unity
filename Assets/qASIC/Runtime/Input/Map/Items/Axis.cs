using qASIC.Input.Devices;
using System;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Axis
    {
        public Axis() { }

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

        public float ReadValue(InputMap map, InputMapData data, IInputDevice device)
        {
            if (IsUsingAxis())
            {
                Input1DAxis axis = map.GetItem<Input1DAxis>(axisGuid);
                return axis?.ReadValue(data, device) ?? 0f;
            }

            InputBinding positive = map.GetItem<InputBinding>(positiveGuid);
            InputBinding negative = map.GetItem<InputBinding>(negativeGuid);

            return (positive?.ReadValue(data, device) ?? 0f) - (negative?.ReadValue(data, device) ?? 0f);
        }

        public InputEventType GetInputEvent(InputMap map, InputMapData data, IInputDevice device)
        {
            if (IsUsingAxis())
            {
                Input1DAxis axis = map.GetItem<Input1DAxis>(axisGuid);
                return axis?.GetInputEvent(data, device) ?? InputEventType.None;
            }

            InputBinding positive = map.GetItem<InputBinding>(positiveGuid);
            InputBinding negative = map.GetItem<InputBinding>(negativeGuid);

            return positive?.GetInputEvent(data, device) ?? InputEventType.None |
                negative?.GetInputEvent(data, device) ?? InputEventType.None;
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