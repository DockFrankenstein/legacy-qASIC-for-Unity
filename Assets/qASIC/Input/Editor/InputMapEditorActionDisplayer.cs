using qASIC.UnityEditor;
using UnityEngine;
using UnityEditor;

namespace qASIC.InputManagement.Internal
{
    public class InputMapEditorActionDisplayer
    {
        public InputGroup group;

        GUIStyle actionStyle = new GUIStyle();

        string newActionName;

        public void OnGUI()
        {
            if (group == null) return;

            actionStyle = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = qGUIUtility.GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f)),
                },
                stretchWidth = true,
            };

            DisplayAddBar();

            for (int i = 0; i < group.actions.Count; i++)
                DisplayAction(i);
        }

        void DisplayAddBar()
        {
            GUILayout.BeginHorizontal();

            newActionName = EditorGUILayout.TextField(newActionName);

            if (GUILayout.Button("Add") && !string.IsNullOrWhiteSpace(newActionName))
                group.actions.Add(new InputAction(newActionName));

            GUILayout.EndHorizontal();
        }

        void DisplayAction(int i)
        {
            InputAction action = group.actions[i];

            GUILayout.BeginHorizontal(actionStyle);
            if (GUILayout.Button(string.IsNullOrWhiteSpace(action.acionName) ? "NULL" : action.acionName))
                group.currentEditorSelectedAction = i;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-"))
                group.actions.RemoveAt(i);

            GUILayout.EndHorizontal();
        }
    }
}