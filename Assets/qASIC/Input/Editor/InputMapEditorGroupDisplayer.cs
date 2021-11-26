using UnityEngine;
using qASIC.UnityEditor;
using UnityEditor;

namespace qASIC.InputManagement.Internal
{
    public class InputMapEditorGroupDisplayer
    {
        public InputMap map;

        GUIStyle groupStyle = new GUIStyle();

        string newGroupName;

        public void OnGUI()
        {
            if (map == null) return;

            groupStyle = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = qGUIUtility.GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f)),
                },
                stretchWidth = true,
            };

            GUILayout.BeginHorizontal();

            newGroupName = EditorGUILayout.TextField(newGroupName);

            if (GUILayout.Button("Add"))
                map.Groups.Add(new InputGroup(newGroupName));

            GUILayout.EndHorizontal();

            for (int i = 0; i < map.Groups.Count; i++)
                DisplayGroup(i);
        }

        void DisplayGroup(int i)
        {
            InputGroup group = map.Groups[i];

            GUILayout.BeginHorizontal(groupStyle);
            if (GUILayout.Button(string.IsNullOrWhiteSpace(group.groupName) ? "NULL" : group.groupName))
                map.currentEditorSelectedGroup = i;

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-"))
                map.Groups.RemoveAt(i);

            GUILayout.EndHorizontal();
        }
    }
}
