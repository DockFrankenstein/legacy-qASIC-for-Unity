using UnityEditor;
using UnityEngine;
using System;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputMapGroupBar
    {
        public InputMap map;

        int scrollIndex;

        public event Action<object> OnItemSelect;

        public void OnGUI()
        {
            BeginHorizontal(EditorStyles.toolbar, Height(EditorGUIUtility.singleLineHeight));

            MoveButton('<', -1, scrollIndex - 1 >= 0);

            EditorGUILayout.BeginScrollView(Vector2.zero, GUIStyle.none, GUIStyle.none);
            BeginHorizontal(EditorStyles.toolbar);

            DisplayGroups();

            if (Button("+", EditorStyles.toolbarButton, Width(EditorGUIUtility.singleLineHeight)))
                CreateGroup();

            EndHorizontal();
            EditorGUILayout.EndScrollView();
            MoveButton('>', 1, map && scrollIndex + 1 < map.Groups.Count);

            EndHorizontal();
        }

        void MoveButton(char character, int value, bool canScroll)
        {
            EditorGUI.BeginDisabledGroup(!map || !canScroll);
            if (Button(character.ToString(), EditorStyles.toolbarButton, Width(EditorGUIUtility.singleLineHeight)))
                scrollIndex += value;
            EditorGUI.EndDisabledGroup();
        }

        public void CreateGroup()
        {
            if (!map) return;

            string name = "New group ";
            int index;
            for (index = 0; map.GroupExists($"{name}{index}"); index++) { }
            map.Groups.Add(new InputGroup($"{name}{index}"));
            Select(map.Groups.Count - 1);
        }

        public void DisplayGroups()
        {
            if (!map) return;

            for (int i = scrollIndex; i < map.Groups.Count; i++)
            {
                EditorStyles.toolbarButton.CalcMinMaxWidth(new GUIContent(map.Groups[i].groupName), out float width, out _);
                if (Toggle(map.currentEditorSelectedGroup == i, map.Groups[i].groupName, EditorStyles.toolbarButton, Width(width)) != (map.currentEditorSelectedGroup == i))
                    Select(i);
            }
        }

        public void Select(int i)
        {
            map.currentEditorSelectedGroup = i;
            OnItemSelect?.Invoke(map.Groups[i]);
        }

        public void ResetScroll() =>
            scrollIndex = 0;
    }
}
