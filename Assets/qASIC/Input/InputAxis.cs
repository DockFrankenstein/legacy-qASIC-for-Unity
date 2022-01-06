using System;
using qASIC.Tools;

namespace qASIC.InputManagement
{
    [Serializable]
    public class InputAxis : INonRepeatable
    {
        public string axisName;

        public string positiveAction;
        public string negativeAction;

        public string ItemName { get => axisName; set => axisName = value; }

        public InputAxis() { }
        public InputAxis(string name)
        {
            axisName = name;
        }
    }
}