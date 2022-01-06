using System;
using System.Collections.Generic;
using UnityEngine;
using qASIC.Tools;

namespace qASIC.InputManagement
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
            action = null;

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].actionName != actionName) continue;
                action = actions[i];
                return true;
            }

            if (logError)
                qDebug.LogError($"Group <b>{groupName}</b> does not contain action <b>{actionName}</b>");

            return false;
        }

        public InputAction GetAction(string actionName)
        {
            TryGetAction(actionName, out InputAction action, true);
            return action;
        }

        public bool TryGetAxis(string axisName, out InputAxis axis, bool logError = false)
        {
            axis = null;

            for (int i = 0; i < axes.Count; i++)
            {
                if (axes[i].axisName != axisName) continue;
                axis = axes[i];
                return true;
            }

            if (logError)
                qDebug.Log($"Group <b>{groupName}</b> does not contain axis <b>{axisName}</b>");

            return false;
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
    }
}