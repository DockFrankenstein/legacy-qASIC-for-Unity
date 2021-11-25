using System;
using System.Collections.Generic;

namespace qASIC.InputManagement
{
    [Serializable]
    public class InputGroup : INonRepeatable
    {
#if UNITY_EDITOR
        public int currentEditorSelectedAction = -1;
#endif

        public string Name { get; set; }

        public List<InputAction> actions = new List<InputAction>();

        public InputGroup() { }

        public InputGroup(string name)
        {
            Name = name;
        }

        public bool TryGetAction(string actionName, out InputAction action, bool logError = false)
        {
            action = null;

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Name != actionName) continue;
                action = actions[i];
                return true;
            }

            if (logError)
                qDebug.LogError($"Group <b>{Name}</b> does not contain group <b>{actionName}</b>");

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
                if (names.Contains(actions[i].Name))
                {
                    qDebug.LogError($"There are multiple actions in the group, cannot index action <b>{actions[i].Name}</b>");
                    continue;
                }

                names.Add(actions[i].Name);
            }
        }

        public override string ToString() =>
            Name;
    }
}