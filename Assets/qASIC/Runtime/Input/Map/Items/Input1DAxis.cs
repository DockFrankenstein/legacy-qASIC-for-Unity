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

        public override float ReadValue(Func<string, float> func) =>
            new Axis(positiveGuid, negativeGuid).ReadValue(map, func);

        public override InputEventType GetInputEvent(Func<string, InputEventType> func) =>
            new Axis(positiveGuid, negativeGuid).GetInputEvent(map, func);

        public override float GetHighestValue(float a, float b) =>
            Mathf.Abs(a) > Mathf.Abs(b) ? a : b;

        public override bool HasErrors() =>
            InputMapUtility.IsGuidBroken<InputBinding>(map, positiveGuid) ||
            InputMapUtility.IsGuidBroken<InputBinding>(map, negativeGuid);
    }
}