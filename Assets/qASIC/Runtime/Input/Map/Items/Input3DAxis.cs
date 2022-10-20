﻿using System;
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

        public override Vector3 ReadValue(Func<string, float> func) =>
            new Vector3(XAxis.ReadValue(map, func), YAxis.ReadValue(map, func), ZAxis.ReadValue(map, func));

        public override InputEventType GetInputEvent(Func<string, InputEventType> func) =>
            XAxis.GetInputEvent(map, func) |
            YAxis.GetInputEvent(map, func);

        public override Vector3 GetHighestValue(Vector3 a, Vector3 b) =>
            a.magnitude > b.magnitude ? a : b;

        public override bool HasErrors() =>
            XAxis.HasErrors(map) ||
            YAxis.HasErrors(map) ||
            ZAxis.HasErrors(map);
    }
}