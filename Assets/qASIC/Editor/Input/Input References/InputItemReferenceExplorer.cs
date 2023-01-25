#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.Input.Map;
using qASIC.Input.Map.Internal;
using System.Collections.Generic;
using qASIC.Tools;
using System;
using System.Linq;

using static UnityEditor.EditorGUILayout;
using Manager = qASIC.Input.Internal.EditorInputManager;
using UnityEngine.UIElements;

namespace qASIC.Input.Internal.ReferenceExplorers
{
    public class InputItemReferenceExplorer : EditorWindow
    {
        protected SerializedProperty contentProperty;

        protected InputMapGroupBar groupBar = new InputMapGroupBar();

        Vector2 _contentScroll;
        int _selectedItemIndex;
        InputMapItem _selectedItem;

        Action<string> OnItemSelected { get; set; }
        string BindingGuid { get; set; }

        static void OpenWindowDebug()
        {
            GetEditorWindow().Show();
        }

        public static void OpenSelectWindow(string currentGuid, Action<string> onItemSelect)
        {
            InputItemReferenceExplorer window = CreateInstance(typeof(InputItemReferenceExplorer)) as InputItemReferenceExplorer;
            OpenWindow(window);
            window.OnItemSelected = onItemSelect;
            window.BindingGuid = currentGuid;
            window.ResetEditor();
        }

        protected static void OpenWindow(EditorWindow window)
        {
            window.minSize = new Vector2(300f, 400f);
            window.titleContent = new GUIContent("Input Reference Explorer");
            window.ShowAuxWindow();
        }

        public static InputItemReferenceExplorer GetEditorWindow() =>
            (InputItemReferenceExplorer)GetWindow(typeof(InputItemReferenceExplorer), false, "Input Map Reference Explorer");

        protected void ResetEditor()
        {
            groupBar.ResetScroll();

            groupBar.OnItemSelect += g =>
            {
                _selectedItemIndex = -1;
            };

            SelectCurrentProperties();
        }

        void SelectCurrentProperties()
        {
            if (!Manager.Map || Manager.Map.groups.Count == 0) return;

            int index = 0;
            if (Manager.Map.ItemsDictionary.ContainsKey(BindingGuid))
            {
                var item = Manager.Map.ItemsDictionary[BindingGuid];
                index = Manager.Map.groups.IndexOf(x => x.items.Contains(item));
            }

            groupBar.Select(index);

            _selectedItemIndex = groupBar.GetSelectedGroup()?.items.IndexOf(Manager.Map.ItemsDictionary[BindingGuid]) ?? -1;
        }

        public void OnGUI()
        {
            if (!Manager.Map)
            {
                HelpBox("Input Map not loaded - Please select an Input Map in project settings.", MessageType.Warning);
                if (GUILayout.Button("Open project settings"))
                    SettingsService.OpenProjectSettings("Project/qASIC/Input");
                return;
            }

            groupBar.SetMap(Manager.Map);

            groupBar.OnGUI();

            DisplayContent();

            Space();
            EditorGUI.BeginDisabledGroup(_selectedItemIndex == -1);
            bool apply = GUILayout.Button("Apply");
            EditorGUI.EndDisabledGroup();

            Event e = Event.current;

            if (e.isKey && e.keyCode == KeyCode.Return && _selectedItemIndex != -1)
                apply = true;

            KeyEvent(KeyCode.UpArrow, _selectedItemIndex - 1 >= 0, () => { _selectedItemIndex--; });
            KeyEvent(KeyCode.DownArrow, _selectedItemIndex + 1 < groupBar.GetSelectedGroup().items.Count(), () => { _selectedItemIndex++; });
            KeyEvent(KeyCode.LeftArrow, true, groupBar.SelectPrevious);
            KeyEvent(KeyCode.RightArrow, true, groupBar.SelectNext);

            if (apply)
            {
                BindingGuid = _selectedItem.Guid;
                OnItemSelected?.Invoke(BindingGuid);
                Close();
            }
        }

        void KeyEvent(KeyCode key, bool condition, Action action)
        {
            if (Event.current.type != EventType.KeyDown || Event.current.keyCode != key || !condition) return;
            action?.Invoke();
            Repaint();
        }

        void DisplayContent()
        {
            List<InputMapItem> content = groupBar
                .GetSelectedGroup()?.items ?? new List<InputMapItem>();
            List<InputMapItem> bindings = content
                .Where(x => x is InputBinding)
                .ToList();
            List<InputMapItem> items = content
                .Where(x => !bindings.Contains(x))
                .ToList();

            _contentScroll = BeginScrollView(_contentScroll);

            _selectedItemIndex = DisplayList(bindings, "Bindings", _selectedItemIndex);
            _selectedItemIndex = DisplayList(items, "Others", _selectedItemIndex - bindings.Count) + bindings.Count;

            if (content.IndexInRange(_selectedItemIndex))
                _selectedItem = _selectedItemIndex < bindings.Count ? bindings[_selectedItemIndex] : items[_selectedItemIndex - bindings.Count];

            EndScrollView();
        }

        int DisplayList(List<InputMapItem> list, string header, int index)
        {
            GUILayout.Label(header);
            using (new GUILayout.VerticalScope(Styles.ItemsGroupStyle))
            {
                GUIContent[] names = list
                    .Select(x => new GUIContent(x.ItemName))
                    .ToArray();

                int newIndex = GUILayout.SelectionGrid(index, names, 1, Styles.ListItemStyle);

                if (newIndex != -1)
                    index = newIndex;
            }

            return index;
        }

        static class Styles
        {
            public static GUIStyle ListItemStyle => new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, };
            public static GUIStyle ItemsGroupStyle => new GUIStyle() { margin = new RectOffset(16, 0, 0, 0), };
        }
    }
}

#endif