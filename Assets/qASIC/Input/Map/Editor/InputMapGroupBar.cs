#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using qASIC.EditorTools;
using qASIC.EditorTools.Internal;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapGroupBar
    {
        public InputMap Map { get; private set; }

        public virtual int SelectedGroupIndex { get; set; }
        public virtual bool EnableContextMenus => false;

        int scrollIndex;

        public event Action<object> OnItemSelect;

        public bool drawToolbarBackground = true;

        public InputGroup GetSelectedGroup() =>
            Map && SelectedGroupIndex >= 0 && SelectedGroupIndex < Map.Groups.Count ? Map.Groups[SelectedGroupIndex] : null;

        public void SetMap(InputMap map) =>
            Map = map;

        #region GUI
        public void OnGUI()
        {
            BeginHorizontal(drawToolbarBackground ? EditorStyles.toolbar : GUIStyle.none, Height(EditorGUIUtility.singleLineHeight));

            MoveButton('<', -1, scrollIndex - 1 >= 0);

            EditorGUILayout.BeginScrollView(Vector2.zero, GUIStyle.none, GUIStyle.none);
            BeginHorizontal(EditorStyles.toolbar);

            DisplayGroups();

            OnListGUI();

            EndHorizontal();
            EditorGUILayout.EndScrollView();
            MoveButton('>', 1, Map && scrollIndex + 1 < Map.Groups.Count);

            EndHorizontal();
        }

        protected virtual void OnListGUI() { }

        void MoveButton(char character, int value, bool canScroll)
        {
            EditorGUI.BeginDisabledGroup(!Map || !canScroll);
            if (Button(character.ToString(), EditorStyles.toolbarButton, Width(EditorGUIUtility.singleLineHeight)))
                scrollIndex += value;
            EditorGUI.EndDisabledGroup();
        }

        public void SelectPrevious()
        {
            int newValue = SelectedGroupIndex - 1;
            if (newValue < 0) return;
            Select(newValue);
        }

        public void SelectNext()
        {
            int newValue = SelectedGroupIndex + 1;
            if (newValue >= Map.Groups.Count) return;
            Select(newValue);
        }

        Rect[] _buttonRects = new Rect[0];
        int _groupContextMenuToOpenOnRepaint = -1;

        public void DisplayGroups()
        {
            if (!Map) return;

            if (Map.Groups.Count != _buttonRects.Length)
                _buttonRects = new Rect[Map.Groups.Count];

            for (int i = scrollIndex; i < Map.Groups.Count; i++)
            {
                //calc width
                EditorStyles.toolbarButton.CalcMinMaxWidth(new GUIContent(Map.Groups[i].groupName), out float width, out _);

                bool isSelected = SelectedGroupIndex == i;
                bool pressed = Toggle(isSelected, Map.Groups[i].groupName, EditorStyles.toolbarButton, Width(width)) != isSelected;

                Event e = Event.current;
                Rect buttonRect = GUILayoutUtility.GetLastRect();

                //Once again I have to use this jank from the toolbar
                //FIXME: fix this garbage
                if (buttonRect.width > 1)
                    _buttonRects[i] = buttonRect;

                //Draw bar
                if (e.type == EventType.Repaint && Map.defaultGroup == i)
                    Styles.DefaultGroupBar.Draw(buttonRect.ResizeToBottom(2f), GUIContent.none, false, false, false, false);

                //Context menu
                //We have to wait for the next repaint before showing the menu as some items can
                //still be selected
                bool showContextMenu = e.type == EventType.Repaint && _groupContextMenuToOpenOnRepaint == i;
                if (showContextMenu)
                {
                    pressed = true;
                    OpenContextMenu(i);
                    _groupContextMenuToOpenOnRepaint = -1;
                }

                if (EnableContextMenus && !showContextMenu && _buttonRects[i].Contains(e.mousePosition) && e.button == 1)
                {
                    _groupContextMenuToOpenOnRepaint = i;
                    pressed = true;
                }

                //Handling group change
                if (pressed)
                    Select(i);
            }
        }
        #endregion

        public virtual void OpenContextMenu(int groupIndex) { }

        #region Deletion
        public void DeleteGroup(InputGroup group)
        {
            if (!Map) return;
            int index = Map.Groups.IndexOf(group);
            if (index == -1) return;

            DeleteGroup(index);
        }

        public virtual void DeleteGroup(int index)
        {
            if (!Map) return;
            Debug.Assert(index >= 0 && index < Map.Groups.Count, $"Cannot delete group {index}, index is out of range!");
            Map.Groups.RemoveAt(index);

            HandleDeleteGroup(index);

            ResetScroll();
        }

        void HandleDeleteGroup(int index)
        {
            if (Map.Groups.Count > 0)
            {
                int selectIndex = Mathf.Max(0, index - 1);
                Select(selectIndex);
                if (Map.defaultGroup == index)
                    SetAsDefault(selectIndex);
                return;
            }

            OnItemSelect?.Invoke(null);
        }
        #endregion

        public virtual void SetAsDefault(int index)
        {
            if (!Map) return;
            Debug.Assert(index >= 0 && index < Map.Groups.Count, $"Cannot set group {index} as default, index is out of range!");
            Map.defaultGroup = index;
        }

        public virtual void Select(int i)
        {
            if (!Map) return;
            SelectedGroupIndex = i;
            OnItemSelect?.Invoke(Map.Groups[i]);
        }

        #region Adding
        public void Add() =>
            Add(-1);

        public void Add(int index) =>
            Add(index, new InputGroup(GenerateUniqueName()));

        public string GenerateUniqueName() =>
            InputMapWindowEditorUtility.GenerateUniqueName("New group", Map.GroupExists);

        public void Add(InputGroup group) =>
            Add(-1, group);

        public void Add(InputGroup selectedGroup, InputGroup group) =>
            Add(Map ? Map.Groups.IndexOf(selectedGroup) : -1, group);

        public virtual void Add(int index, InputGroup group)
        {
            if (!Map) return;

            //If index is out of range, add the item at the end
            if (index < 0 || index >= Map.Groups.Count)
                index = Map.Groups.Count - 1;

            Map.Groups.Insert(index + 1, group);
            Select(index + 1);
        }
        #endregion

        public void ResetScroll() =>
            scrollIndex = 0;

        static class Styles
        {
            public static GUIStyle DefaultGroupBar => new GUIStyle().WithBackground(qGUIUtility.GenerateColorTexture(InputMapWindow.Prefs_DefaultGroupColor));
        }

    }
}

#endif