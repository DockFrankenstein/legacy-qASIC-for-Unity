using UnityEngine;
using TMPro;

namespace qASIC.InputManagement.DebugTools
{
    public class InputList : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        private void Update()
        {
            if (text == null) return;
            ResetText();

            if (!InputManager.MapLoaded)
            {
                text.text = "Map not assigned";
                return;
            }

            foreach (InputGroup group in InputManager.Map.Groups)
            {
                Debug.Log("asd");
                AddLine(group.groupName);
                foreach(InputAction action in group.actions)
                {
                    AddLine($"├{action.actionName}: {action.GetInput()}");
                    for (int i = 0; i < action.keys.Count; i++)
                    {
                        KeyCode key = InputManager.GetKeyCode(group.groupName, action.actionName, i);
                        AddLine($"├─{key}: {Input.GetKey(key).ToStringFormated()}");
                    }
                }
            }
        }

        void AddLine(string line)
        {
            if (text.text == null) return;
            text.text += $"{line}\n";
        }

        void ResetText()
        {
            if (text.text == null) return;
            text.text = string.Empty;
        }
    }
}