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
                Add();

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

        Rect[] _buttonRects = new Rect[0];
        int _groupContextMenuToOpenOnRepaint = -1;

        public void DisplayGroups()
        {
            if (!map) return;

            if (map.Groups.Count != _buttonRects.Length)
                _buttonRects = new Rect[map.Groups.Count];

            for (int i = scrollIndex; i < map.Groups.Count; i++)
            {
                //calc width
                EditorStyles.toolbarButton.CalcMinMaxWidth(new GUIContent(map.Groups[i].groupName), out float width, out _);

                bool isSelected = map.currentEditorSelectedGroup == i;
                bool pressed = Toggle(isSelected, map.Groups[i].groupName, EditorStyles.toolbarButton, Width(width)) != isSelected;
                
                Event e = Event.current;
                Rect buttonRect = GUILayoutUtility.GetLastRect();

                //Once again I have to use this jank from the toolbar
                //FIXME: fix this garbage
                if (buttonRect.width > 1)
                    _buttonRects[i] = buttonRect;

                //Draw bar
                if (e.type == EventType.Repaint && map.defaultGroup == i)
                    Styles.defaultGroupBar.Draw(buttonRect.ResizeToBottom(2f), GUIContent.none, false, false, false, false);

                //Context menu
                //We have to wait for the next repaint before showing the menu as some items can
                //still be selected
                bool showContextMenu = e.type == EventType.Repaint && _groupContextMenuToOpenOnRepaint == i;
                if (showContextMenu)
                {
                    pressed = true;
                    GenericMenu menu = new GenericMenu();
                    int index = i;
                    menu.AddToggableItem("Set as default", false, () => SetAsDefault(index), map.defaultGroup != i);
                    menu.AddItem("Add", false, () => Add(index));
                    menu.AddItem("Delete", false, () => DeleteGroup(index));
                    menu.ShowAsContext();
                    _groupContextMenuToOpenOnRepaint = -1;
                }

                if (!showContextMenu && _buttonRects[i].Contains(e.mousePosition) && e.button == 1)
                {
                    _groupContextMenuToOpenOnRepaint = i;
                    pressed = true;
                }

                //Handling group change
                if (pressed)
                     Select(i);
            }
        }

        public void DeleteGroup(InputGroup group)
        {
            if (!map) return;
            int index = map.Groups.IndexOf(group);
            if (index == -1) return;

            DeleteGroup(index);
        }

        public void DeleteGroup(int index)
        {
            if (!map) return;
            Debug.Assert(index >= 0 && index < map.Groups.Count, $"Cannot delete group {index}, index is out of range!");
            map.Groups.RemoveAt(index);

            HandleDeleteGroup(index);

            ResetScroll();
            InputMapWindow.SetMapDirty();
        }

        void HandleDeleteGroup(int index)
        {
            if (map.Groups.Count > 0)
            {
                int selectIndex = Mathf.Max(0, index - 1);
                Select(selectIndex);
                if (map.defaultGroup == index)
                    SetAsDefault(selectIndex);
                return;
            }

            OnItemSelect?.Invoke(null);
        }

        public void SetAsDefault(int index)
        {
            if (!map) return;
            Debug.Assert(index >= 0 && index < map.Groups.Count, $"Cannot set group {index} as default, index is out of range!");
            map.defaultGroup = index;
            InputMapWindow.SetMapDirty();
        }

        public void Select(int i)
        {
            if (!map) return;
            map.currentEditorSelectedGroup = i;
            OnItemSelect?.Invoke(map.Groups[i]);
        }

        public void Add() =>
            Add(-1);

        public void Add(int index) =>
            Add(index, new InputGroup(InputMapEditorUtility.GenerateUniqueName("New group", map.GroupExists)));

        public void Add(InputGroup group) =>
            Add(-1, group);

        public void Add(InputGroup selectedGroup, InputGroup group) =>
            Add(map ? map.Groups.IndexOf(selectedGroup) : -1, group);

        public void Add(int index, InputGroup group)
        {
            if (!map) return;

            //If index is out of range, add the item at the end
            if (index < 0 || index >= map.Groups.Count)
                index = map.Groups.Count - 1;

            map.Groups.Insert(index + 1, group);
            Select(index + 1);

            InputMapWindow.SetMapDirty();
        }

        public void ResetScroll() =>
            scrollIndex = 0;

        static class Styles
        {
            public static GUIStyle defaultGroupBar = new GUIStyle().WithBackground(qGUIUtility.qASICColorTexture);
        }
    }
}
