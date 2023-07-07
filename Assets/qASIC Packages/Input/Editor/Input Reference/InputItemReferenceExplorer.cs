#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.Input.Map;
using qASIC.Input.Map.Internal;
using System.Collections.Generic;
using qASIC.EditorTools;
using System;
using System.Linq;

using static UnityEditor.EditorGUILayout;
using Manager = qASIC.Input.Internal.EditorInputManager;

namespace qASIC.Input.Internal
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
        InputMap Map { get; set; }
        Type ItemType { get; set; } = typeof(InputMapItem);

        static void OpenWindowDebug()
        {
            GetEditorWindow().Show();
        }

        public static void OpenSelectWindow(string currentGuid, Action<string> onItemSelect) =>
            OpenSelectWindow(currentGuid, onItemSelect, typeof(InputMapItem));

        public static void OpenSelectWindow<T>(string currentGuid, Action<string> onItemSelect) where T : InputMapItem =>
            OpenSelectWindow(currentGuid, onItemSelect, typeof(T));

        public static void OpenSelectWindow(string currentGuid, Action<string> onItemSelect, Type itemType) =>
            OpenSelectWindow(Manager.Map, currentGuid, onItemSelect);


        public static void OpenSelectWindow(InputMap map, string currentGuid, Action<string> onItemSelect) =>
            OpenSelectWindow(map, currentGuid, onItemSelect, typeof(InputMapItem));

        public static void OpenSelectWindow<T>(InputMap map, string currentGuid, Action<string> onItemSelect) where T : InputMapItem =>
            OpenSelectWindow(map, currentGuid, onItemSelect, typeof(T));

        public static void OpenSelectWindow(InputMap map, string currentGuid, Action<string> onItemSelect, Type type)
        {
            InputItemReferenceExplorer window = CreateInstance(typeof(InputItemReferenceExplorer)) as InputItemReferenceExplorer;
            OpenWindow(window);
            window.OnItemSelected = onItemSelect;
            window.BindingGuid = currentGuid;
            window.Map = map;
            window.ItemType = type;
            window.groupBar.SetMap(map);

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
            if (Map == null || Map.groups.Count == 0) return;

            _selectedItem = Map.GetItem<InputMapItem>(BindingGuid);

            groupBar.Select(_selectedItem == null ?
                0 :
                Map.groups.IndexOf(x => x.items.Contains(_selectedItem)));

            _selectedItemIndex = groupBar.GetSelectedGroup()?.items.IndexOf(_selectedItem) ?? -1;
        }

        public void OnGUI()
        {
            if (!Map)
            {
                HelpBox("Input Map not loaded - Please select an Input Map in project settings.", MessageType.Warning);
                if (GUILayout.Button("Open project settings"))
                    SettingsService.OpenProjectSettings("Project/qASIC/Input");
                return;
            }

            groupBar.SetMap(Map);

            groupBar.OnGUI();

            DisplayContent();

            qGUIEditorUtility.HorizontalLineLayout();
            if (GUILayout.Button("Add Items"))
            {
                InputMapWindow.OpenMap(Map);
                Close();
            }

            bool apply = GUILayout.Button("Apply", GUILayout.Height(26f));

            Event e = Event.current;

            if (e.isKey && e.keyCode == KeyCode.Return && _selectedItemIndex != -1)
                apply = true;

            KeyEvent(KeyCode.UpArrow, _selectedItemIndex - 1 >= 0, () => { _selectedItemIndex--; });
            KeyEvent(KeyCode.DownArrow, _selectedItemIndex + 1 < groupBar.GetSelectedGroup().items.Count(), () => { _selectedItemIndex++; });
            KeyEvent(KeyCode.LeftArrow, true, groupBar.SelectPrevious);
            KeyEvent(KeyCode.RightArrow, true, groupBar.SelectNext);

            if (apply)
            {
                BindingGuid = _selectedItemIndex == -1 ? string.Empty : _selectedItem.Guid;
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
                .GetSelectedGroup()?.items
                .Where(x => ItemType.IsAssignableFrom(x.GetType()))
                .ToList() 
                ?? new List<InputMapItem>();
            
            var bindings = content
                .Where(x => x is InputBinding)
                .ToArray();

            var items = content
                .Except(bindings)
                .ToArray();

            _contentScroll = BeginScrollView(_contentScroll);

            if (DisplayList(new GUIContent[] { new GUIContent("None") }, string.Empty, _selectedItemIndex + 1) == 0)
                _selectedItemIndex = -1;

            if (ItemType.IsAssignableFrom(typeof(InputBinding)))
                _selectedItemIndex = DisplayList(bindings, "Bindings", _selectedItemIndex);

            if (!typeof(InputBinding).IsAssignableFrom(ItemType))
                _selectedItemIndex = DisplayList(items, "Others", _selectedItemIndex - bindings.Length) + bindings.Length;

            if (content.IndexInRange(_selectedItemIndex))
                _selectedItem = _selectedItemIndex < bindings.Length ? bindings[_selectedItemIndex] : items[_selectedItemIndex - bindings.Length];

            EndScrollView();
        }

        int DisplayList(InputMapItem[] list, string header, int index) =>
            DisplayList(list
                .Select(x => new GUIContent(x.ItemName))
                .ToArray(), header, index);

        int DisplayList(GUIContent[] list, string header, int index)
        {
            bool containsHeader = !string.IsNullOrEmpty(header);

            if (containsHeader)
            {
                GUILayout.Space(8f);
                GUILayout.Label(header, Styles.HeaderStyle);
            }

            using (new GUILayout.VerticalScope(containsHeader ? Styles.ItemsHeaderGroupStyle : Styles.ItemsGroupStyle))
            {
                using (new GUILayout.VerticalScope(GUILayout.Height((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * list.Length)))
                    GUILayout.FlexibleSpace();

                Rect listRect = GUILayoutUtility.GetLastRect();

                int newIndex = GUI.SelectionGrid(listRect, index, list.ToArray(), 1, Styles.ListItemStyle);

                if (newIndex != -1)
                    index = newIndex;
            }

            return index;
        }

        static class Styles
        {
            public static GUIStyle ListItemStyle => new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, };

            public static GUIStyle ItemsHeaderGroupStyle => new GUIStyle() { margin = new RectOffset(16, 0, 0, 0), };
            public static GUIStyle ItemsGroupStyle => new GUIStyle() { margin = new RectOffset(0, 0, 0, 0), };
            public static GUIStyle HeaderStyle => new GUIStyle(EditorStyles.label) 
            { 
                fixedHeight = 18f,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(4, 4, 4, 4),
                
            }.WithBackgroundColor(EditorGUIUtility.isProSkin ? 
                new Color(0.1f, 0.1f, 0.1f) :
                new Color(0.6f, 0.6f, 0.6f));
        }
    }
}

#endif