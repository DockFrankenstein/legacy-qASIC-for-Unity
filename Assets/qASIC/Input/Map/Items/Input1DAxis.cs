using System;

namespace qASIC.InputManagement.Map
{
    [Serializable]
    public class Input1DAxis : InputMapItem<float>
    {
        public Input1DAxis() : base() { }
        public Input1DAxis(string name) : base(name) { }

        public string positiveAction;
        public string negativeAction;

        public override float ReadValue(Func<string, float> func)
        {
            InputBinding positive = map.GetItem<InputBinding>(positiveAction);
            InputBinding negative = map.GetItem<InputBinding>(negativeAction);

            float positiveValue = positive == null ? 0f : positive.ReadValue(func);
            float negativeValue = negative == null ? 0f : negative.ReadValue(func);

            return positiveValue - negativeValue;
        }

        public override bool GetInputEvent(Func<string, bool> func)
        {
            InputBinding positive = map.GetItem<InputBinding>(positiveAction);
            InputBinding negative = map.GetItem<InputBinding>(negativeAction);

            return positive.GetInputEvent(func) || negative.GetInputEvent(func);
        }

        public override float GetHighestValue(float a, float b) =>
            a > b ? a : b;
    }
}