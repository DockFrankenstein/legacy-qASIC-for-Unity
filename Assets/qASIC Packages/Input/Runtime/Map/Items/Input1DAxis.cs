using qASIC.Input.Devices;
using System;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input1DAxis : InputMapItem<float>
    {
        public Input1DAxis() : base() { }
        public Input1DAxis(string name) : base(name) { }

        public string positiveGuid = string.Empty;
        public string negativeGuid = string.Empty;

        public override float ReadValue(InputMapData data, IInputDevice device) =>
            new Axis(positiveGuid, negativeGuid).ReadValue(map, data, device);

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device) =>
            new Axis(positiveGuid, negativeGuid).GetInputEvent(map, data, device);

        public override float GetHighestValue(float a, float b) =>
            Mathf.Abs(a) > Mathf.Abs(b) ? a : b;

        public override bool HasErrors() =>
            InputMapUtility.IsGuidBroken<InputBinding>(map, positiveGuid) ||
            InputMapUtility.IsGuidBroken<InputBinding>(map, negativeGuid);
    }
}