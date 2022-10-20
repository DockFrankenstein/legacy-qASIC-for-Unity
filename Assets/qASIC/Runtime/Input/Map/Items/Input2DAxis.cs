using System;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input2DAxis : InputMapItem<Vector2>
    {
        public Input2DAxis() : base() { }
        public Input2DAxis(string name) : base(name) { }

        public Axis XAxis = new Axis();
        public Axis YAxis = new Axis();

        public override Vector2 ReadValue(Func<string, float> func) =>
            new Vector2(XAxis.ReadValue(map, func), YAxis.ReadValue(map, func));

        public override InputEventType GetInputEvent(Func<string, InputEventType> func) =>
            XAxis.GetInputEvent(map, func) |
            YAxis.GetInputEvent(map, func);

        public override Vector2 GetHighestValue(Vector2 a, Vector2 b) =>
            a.magnitude > b.magnitude ? a : b;

        public override bool HasErrors() =>
            XAxis.HasErrors(map) ||
            YAxis.HasErrors(map);
    }
}