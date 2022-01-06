using UnityEditor;
using UnityEngine;
using System;
using qASIC.EditorTools;

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
            map.Groups.Add(new InputGroup(InputMapEditorUtility.GenerateUniqueName("New group", map.GroupExists)));
            Select(map.Groups.Count - 1);
        }

        public void DisplayGroups()
        {
            if (!map) return;

            for (int i = scrollIndex; i < map.Groups.Count; i++)
            {
                //calc width
                EditorStyles.toolbarButton.CalcMinMaxWidth(new GUIContent(map.Groups[i].groupName), out float width, out _);

                bool isSelected = map.currentEditorSelectedGroup == i;
                bool pressed = Toggle(isSelected, map.Groups[i].groupName, EditorStyles.toolbarButton, Width(width)) != isSelected;

                //Draw bar
                if (Event.current.type == EventType.Repaint && map.defaultGroup == i)
                    Styles.defaultGroupBar.Draw(GUILayoutUtility.GetLastRect().ResizeToBottom(2f), GUIContent.none, false, false, false, false);

                if (pressed)
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

        static class Styles
        {
            public static GUIStyle defaultGroupBar = new GUIStyle().WithBackground(qGUIUtility.qASICColorTexture);
        }
    }
}
