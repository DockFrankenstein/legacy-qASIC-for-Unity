using System;
using qASIC.Tools;

namespace qASIC.InputManagement.Map
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

        public bool NameEquals(string name) =>
            NonRepeatableChecker.Compare(axisName, name);
    }
}