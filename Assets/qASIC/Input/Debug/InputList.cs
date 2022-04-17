using UnityEngine;
using TMPro;
using qASIC.InputManagement.Map;

namespace qASIC.InputManagement.DebugTools
{
    [AddComponentMenu("qASIC/Input/Input List")]
    public class InputList : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        public void SetupText(TMP_Text text) =>
            this.text = text;

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
                AddLine(group.groupName);
                foreach(InputAction action in group.actions)
                {
                    AddLine($"├{action.actionName}: {action.GetInput()}");
                    for (int i = 0; i < action.keys.Count; i++)
                    {
                        KeyCode key = InputManager.GetKeyCode(group.groupName, action.actionName, i);
                        AddLine($"├─{key}: {Input.GetKey(key).ToStringFormatted()}");
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