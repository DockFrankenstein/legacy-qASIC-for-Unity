using UnityEngine;

namespace qASIC.InputManagement
{
    [System.Serializable]
    public class InputAxisReference
    {
        [SerializeField] string groupName;
        [SerializeField] bool useDefaultGroup = true;
        [SerializeField] string axisName;

        public string GroupName =>
            useDefaultGroup ? (InputManager.Map ? InputManager.Map.DefaultGroupName : string.Empty) : groupName;
        public string AxisName =>
            axisName;

        public InputAxisReference() { }

        public InputAxisReference(string groupName, string axisName)
        {
            this.groupName = groupName;
            this.axisName = axisName;
        }
    }
}
