using System;

namespace qASIC.InputManagement
{
    [Serializable]
    public class InputAxis : INonRepeatable
    {
        public string axisName;

        public string positiveAction;
        public string negativeAction;

        public string ItemName { get => axisName; }
    }
}