using System;
using UnityEngine;

namespace qASIC.Input.Map
{
    [Serializable]
    public class Input3DAxis : InputMapItem<Vector3>
    {
        public Input3DAxis() : base() { }
        public Input3DAxis(string name) : base(name) { }

        public Axis XAxis = new Axis();
        public Axis YAxis = new Axis();
        public Axis ZAxis = new Axis();

        public override Vector3 ReadValue(InputMapData data, Func<string, float> func) =>
            new Vector3(XAxis.ReadValue(map, data, func), YAxis.ReadValue(map, data, func), ZAxis.ReadValue(map, data, func));

        public override InputEventType GetInputEvent(InputMapData data, Func<string, InputEventType> func) =>
            XAxis.GetInputEvent(map, data, func) |
            YAxis.GetInputEvent(map, data, func);

        public override Vector3 GetHighestValue(Vector3 a, Vector3 b) =>
            a.magnitude > b.magnitude ? a : b;

        public override bool HasErrors() =>
            XAxis.HasErrors(map) ||
            YAxis.HasErrors(map) ||
            ZAxis.HasErrors(map);
    }
}