using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.UnityEditor;
using UnityEditor;

namespace qASIC.InputManagement.Internal
{
    public class InputMapEditorKeysDisplayer
    {
        public InputAction action;

        GUIStyle keyStyle = new GUIStyle();

        public void OnGUI()
        {
            if (action == null) return;

            keyStyle = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = qGUIUtility.GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f)),
                },
                stretchWidth = true,
            };

            DisplayAddBar();

            for (int i = 0; i < action.keys.Count; i++)
                DisplayAction(i);
        }

        void DisplayAddBar()
        {
            if (GUILayout.Button("Add"))
                action.keys.Add(KeyCode.None);
        }

        void DisplayAction(int i)
        {
            GUILayout.BeginHorizontal(keyStyle);
            action.keys[i] = (KeyCode)EditorGUILayout.EnumPopup("Key ", action.keys[i]);

            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                action.keys.RemoveAt(i);

            GUILayout.EndHorizontal();
        }
    }
}