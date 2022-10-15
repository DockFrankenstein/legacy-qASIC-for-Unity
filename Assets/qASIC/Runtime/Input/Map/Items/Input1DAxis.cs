using System;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input1DAxis : InputMapItem<float>
    {
        public Input1DAxis() : base() { }
        public Input1DAxis(string name) : base(name) { }

        public string positiveAction;
        public string negativeAction;

        public override float ReadValue(Func<string, float> func) =>
            new Axis(positiveAction, negativeAction).ReadValue(map, func);

        public override InputEventType GetInputEvent(Func<string, InputEventType> func) =>
            new Axis(positiveAction, negativeAction).GetInputEvent(map, func);

        public override float GetHighestValue(float a, float b) =>
            Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
    }
}