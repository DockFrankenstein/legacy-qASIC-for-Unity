using System.Collections;
using System.Collections.Generic;
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
                    AddLine($"{action.acionName}: {action.GetInput()}", 1);
                    for (int i = 0; i < action.keys.Count; i++)
                    {
                        KeyCode key = InputManager.GetKeyCode(action.acionName, i, group.groupName);
                        AddLine($"{key}: {Input.GetKey(key)}", 2);
                    }
                }
            }
        }

        void AddLine(string line, int index = 0)
        {
            if (text.text == null) return;
            for (int i = 0; i < index; i++)
                text.text += " ";

            text.text += $"{line}\n";
        }

        void ResetText()
        {
            if (text.text == null) return;
            text.text = string.Empty;
        }
    }
}