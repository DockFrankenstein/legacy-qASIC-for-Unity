using UnityEngine;

namespace qASIC.InputManagement
{
    [System.Serializable]
    public class InputActionReference
    {
        [SerializeField] string groupName;
        [SerializeField] bool useDefaultGroup = true;
        [SerializeField] string actionName;

        public string GroupName =>
            useDefaultGroup ? (InputManager.Map ? InputManager.Map.DefaultGroupName : string.Empty) : groupName;
        public string ActionName =>
            actionName;

        public InputActionReference() { }

        public InputActionReference(string groupName, string actionName)
        {
            this.groupName = groupName;
            this.actionName = actionName;
        }
    }
}