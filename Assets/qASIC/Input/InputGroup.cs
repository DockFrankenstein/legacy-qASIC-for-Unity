using System;
using System.Collections.Generic;

namespace qASIC.InputManagement
{
    [Serializable]
    public class InputGroup
    {
#if UNITY_EDITOR
        public int currentEditorSelectedAction = -1;
#endif

        public string groupName;

        public List<InputAction> actions = new List<InputAction>();

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
                if (actions[i].acionName != actionName) continue;
                action = actions[i];
                return true;
            }

            if (logError)
                qDebug.LogError($"Group <b>{groupName}</b> does not contain group <b>{actionName}</b>");

            return false;
        }

        public InputAction GetAction(string actionName)
        {
            TryGetAction(actionName, out InputAction action, true);
            return action;
        }

        public void CheckForRepeatingActions()
        {
            List<string> names = new List<string>();
            for (int i = 0; i < actions.Count; i++)
            {
                if (names.Contains(actions[i].acionName))
                {
                    qDebug.LogError($"There are multiple actions in the group, cannot index action <b>{actions[i].acionName}</b>");
                    continue;
                }

                names.Add(actions[i].acionName);
            }
        }

        public override string ToString() =>
            groupName;

        public bool NameEquals(string name) =>
            groupName.ToLower() == name.ToLower();
    }
}