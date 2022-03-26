using System;
using System.Collections.Generic;
using qASIC.Tools;

namespace qASIC.InputManagement.Map
{
    [Serializable]
    public class InputGroup : INonRepeatable
    {
        public string groupName;

        public List<InputAction> actions = new List<InputAction>();
        public List<InputAxis> axes = new List<InputAxis>();

        public string ItemName { get => groupName; set => groupName = value; }

        public InputGroup() { }

        public InputGroup(string name)
        {
            groupName = name;
        }

        public bool TryGetAction(string actionName, out InputAction action, bool logError = false)
        {
            bool contains = NonRepeatableChecker.TryGetItem(actions, actionName, out action);

            if (!contains && logError)
                qDebug.LogError($"Group '{groupName}' does not contain action '{actionName}'");

            return contains;
        }

        public InputAction GetAction(string actionName)
        {
            TryGetAction(actionName, out InputAction action, true);
            return action;
        }

        public bool TryGetAxis(string axisName, out InputAxis axis, bool logError = false)
        {
            bool contains = NonRepeatableChecker.TryGetItem(axes, axisName, out axis);

            if (!contains && logError)
                qDebug.Log($"Group '{groupName}' does not contain axis '{axisName}'");

            return contains;
        }

        public InputAxis GetAxis(string axisName)
        {
            TryGetAxis(axisName, out InputAxis axis, true);
            return axis;
        }

        public void CheckForRepeating()
        {
            NonRepeatableChecker.LogContainsRepeatable(actions);
            NonRepeatableChecker.LogContainsRepeatable(axes);
        }

        public override string ToString() =>
            groupName;

        public bool NameEquals(string name) =>
            groupName.ToLower() == name.ToLower();

        public bool ActionExists(string actionName)
        {
            for (int i = 0; i < actions.Count; i++)
                if (actions[i]?.NameEquals(actionName) == true)
                    return true;
            return false;
        }

        public bool AxisExists(string axisName)
        {
            for (int i = 0; i < axes.Count; i++)
                if (axes[i]?.NameEquals(axisName) == true)
                    return true;
            return false;
        }

        public bool CanRenameAction(string newName) =>
            !string.IsNullOrWhiteSpace(newName) && !ActionExists(newName);

        public bool CanRenameAxis(string newName) =>
            !string.IsNullOrWhiteSpace(newName) && !AxisExists(newName);
    }
}