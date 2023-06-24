using qASIC.Input.Devices;
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

        public override Vector2 ReadValue(InputMapData data, IInputDevice device) =>
            new Vector2(XAxis.ReadValue(map, data, device), YAxis.ReadValue(map, data, device));

        public override InputEventType GetInputEvent(InputMapData data, IInputDevice device) =>
            XAxis.GetInputEvent(map, data, device) |
            YAxis.GetInputEvent(map, data, device);

        public override Vector2 GetHighestValue(Vector2 a, Vector2 b) =>
            a.magnitude > b.magnitude ? a : b;

        public override bool HasErrors() =>
            XAxis.HasErrors(map) ||
            YAxis.HasErrors(map);
    }
}