using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using qASIC.Input.Internal.KeyProviders;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using qASIC.EditorTools;
using static qASIC.EditorTools.EditorChangeChecker;

namespace qASIC.Input.Map.Internal.Inspectors
{
    public class InputBindingInspector : InputMapItemInspector
    {
        public override Type ItemType => typeof(InputBinding);

        InputBinding _binding;

        ReorderableList _keyPathReorderableList;

        List<KeyTypeList> _keyLists = new List<KeyTypeList>();

        int _keyViewMode;
        bool _pathsErrors;

        public override void Initialize(OnInitializeContext context)
        {
            //Generic inspector initialization

            //Item related initialization
            _binding = context.item as InputBinding;

            //Create key lists for types
            _keyLists = InputMapUtility.KeyTypeProviders
                .Select(GenerateList)
                .ToList();

            _keyPathReorderableList = new ReorderableList(_binding.keys, typeof(string), true, true, true, true);
            _keyPathReorderableList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Key paths");
            _keyPathReorderableList.onAddCallback += _ =>
            {
                _binding.keys.Add(string.Empty);
            };

            _keyPathReorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                string rootPath = _binding.keys[index].Split('/').FirstOrDefault();
                if (InputMapUtility.KeyTypeProviders.Where(x => x.KeyName == rootPath).Count() == 0)
                {
                    Rect errorRect = rect.SetWidth(rect.height);
                    rect = rect.BorderLeft(rect.height + 2f);
                    GUI.Label(errorRect, qGUIEditorUtility.ErrorIcon);
                }

                _binding.keys[index] = EditorGUI.DelayedTextField(rect, _binding.keys[index]);
            };
        }

        KeyTypeList GenerateList(KeyTypeProvider provider)
        {
            string keyName = $"{provider.KeyName}/";

            List<int> indexes = new List<int>();
            List<string> keys = _binding.keys
                .Where((i, x) =>
                {
                    bool b = x.StartsWith(keyName);

                    if (b)
                        indexes.Add(i);

                    return b;
                })
                .ToList();

            var list = new KeyTypeList(provider, keys, indexes);
            list.Initialize();

            list.OnChangeItem += (a, b) => List_OnChangeItem(list, a, b);
            list.reorderableList.onAddCallback += a => ReorderableList_OnAddCallback(list, a);
            list.reorderableList.onRemoveCallback += a => ReorderableList_OnRemoveCallback(list, a);
            list.reorderableList.onReorderCallbackWithDetails += (a, b, c) => ReorderableList_OnReorderCallbackWithDetails(list, a, b, c);

            return list;
        }

        #region List callbacks
        void List_OnChangeItem(KeyTypeList list, int index, string path)
        {
            _binding.keys[index] = $"{list.provider.KeyName}/{path}";
            SetMapDirty();
        }
        #endregion

        #region Reorderable Lists callbacks
        void ReorderableList_OnAddCallback(KeyTypeList list, ReorderableList reorderableList)
        {
            string item = $"{list.provider.KeyName}/";
            _binding.keys.Add(item);
            list.indexes.Add(_binding.keys.Count - 1);
            list.keys.Add(item);
            window.SetMapDirty();
        }

        void ReorderableList_OnRemoveCallback(KeyTypeList list, ReorderableList reorderableList)
        {
            int index = reorderableList.index;
            _binding.keys.RemoveAt(list.indexes[index]);
            list.keys.RemoveAt(index);
            list.indexes.RemoveAt(index);
        }

        void ReorderableList_OnReorderCallbackWithDetails(KeyTypeList list, ReorderableList reorderableList, int oldIndex, int newIndex)
        {
            int trueOldIndex = list.indexes[oldIndex];
            int trueNewIndex = list.indexes[newIndex];
            string temp = _binding.keys[trueNewIndex];
            _binding.keys[trueNewIndex] = _binding.keys[trueOldIndex];
            _binding.keys[trueOldIndex] = temp;
            window.SetMapDirty();
        }
        #endregion

        protected override void OnGUI(OnGUIContext context)
        {
            _pathsErrors = _binding.HasUnassignedPaths().Count != 0;

            DisplayKeys();
        }

        void DisplayKeys()
        {
            EditorGUILayout.Space();

            GUIContent[] toolbarContent = new GUIContent[]
            {
                new GUIContent("Keys"),
                new GUIContent("Raw", _pathsErrors ? qGUIEditorUtility.ErrorIcon : null),
            };

            using (new ChangeCheckPause())
            {
                _keyViewMode = GUILayout.Toolbar(_keyViewMode, toolbarContent, GUILayout.Height(20f));
            }

            EditorGUILayout.Space();

            switch (_keyViewMode)
            {
                default:
                    foreach (var list in _keyLists)
                    {
                        list.Draw();
                        EditorGUILayout.Space();
                    }
                    break;
                case 1:
                    _keyPathReorderableList.DoLayoutList();
                    break;
            }
        }

        protected override void HandleDeletion(OnGUIContext context)
        {
            map.groups
                .Where(x => x.items.Contains(context.item))
                .First()
                .RemoveItem(context.item as InputMapItem);
        }

        private class KeyTypeList
        {
            public KeyTypeList(KeyTypeProvider provider, List<string> keys, List<int> indexes)
            {
                this.provider = provider;
                this.keys = keys;
                this.indexes = indexes;
            }

            public ReorderableList reorderableList;
            public KeyTypeProvider provider;
            public List<string> keys;
            public List<int> indexes;

            public Action<int, string> OnChangeItem;

            public void Initialize()
            {
                reorderableList = new ReorderableList(keys, typeof(string), true, true, true, true);

                reorderableList.drawHeaderCallback += List_DrawHeaderCallback;
                reorderableList.drawElementCallback += List_DrawElementCallback;
            }

            void ShowMenu(Rect rect, int index)
            {
                PopupWindow.Show(rect, new InputBindingSearchPopupContent(provider, index, Popup_OnApply, new Vector2(rect.width, 200f)));
            }

            void List_DrawHeaderCallback(Rect rect)
            {
                GUI.Label(rect, provider.DisplayName);
            }

            void List_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
            {
                if (GUI.Button(rect, keys[index].Split('/').LastOrDefault(), EditorStyles.popup))
                {
                    ShowMenu(rect, index);
                }
            }

            void Popup_OnApply(int index, string path)
            {
                keys[index] = path;
                OnChangeItem?.Invoke(indexes[index], path);
            }

            public void Draw()
            {
                reorderableList.DoLayoutList();
            }

            public void Reload()
            {

            }
        }
    }

    public class InputBindingSearchPopupContent : PopupWindowContent
    {
        public InputBindingSearchPopupContent(KeyTypeProvider provider, int index, Action<int, string> onApply, Vector2 size) : base()
        {
            _size = size;
            _targetItemIndex = index;
            _OnApply = onApply;
            _provider = provider;
        }

        Vector2 _size;
        KeyTypeProvider _provider;

        Vector2 _scroll;
        string _search = string.Empty;
        int _index = 0;

        public override Vector2 GetWindowSize() =>
            _size;

        List<string> _items;
        List<string> _currentItems;
        Action<int, string> _OnApply;
        int _targetItemIndex;
        bool _keyDown;

        SearchField _searchField = new SearchField();

        public override void OnOpen()
        {
            base.OnOpen();
            _items = _provider.GetKeyList().ToList();
            _currentItems = _items;
        }

        public override void OnGUI(Rect rect)
        {
            DrawTopBar();
            EditorGUILayout.Space(8f);
            qGUIEditorUtility.HorizontalLine();

            _currentItems = qGUIEditorUtility.SortSearchList(_items, _search);

            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scroll))
            {
                _scroll = scrollView.scrollPosition;
                DrawTree(_currentItems);
            }

            qGUIEditorUtility.HorizontalLine();
            GUILayout.Space(16f);


            if (GUILayout.Button("Select"))
                Apply();

            if (Event.current.rawType == EventType.Used)
            {
                if (_keyDown)
                    return;

                _keyDown = true;

                switch (Event.current.keyCode)
                {
                    case KeyCode.DownArrow:
                        _index = Mathf.Clamp(_index + 1, 0, _currentItems.Count - 1);
                        if (_index != 0)
                            _scroll.y += 20f;

                        editorWindow.Repaint();
                        break;
                    case KeyCode.UpArrow:
                        _index = Mathf.Clamp(_index - 1, 0, _currentItems.Count - 1);
                        _scroll.y = Mathf.Max(_scroll.y - 20f, 0f);
                        editorWindow.Repaint();
                        break;
                    case KeyCode.Return:
                        Apply();
                        break;
                }

                return;
            }

            _keyDown = false;
        }

        void Apply()
        {
            editorWindow.Close();
            _OnApply?.Invoke(_targetItemIndex, _index == -1 ? string.Empty : _currentItems[_index]);
        }

        void DrawTopBar()
        {
            EditorGUILayout.Space(16f);
            Rect totalRect = GUILayoutUtility.GetLastRect().Border(4f).SetHeight(16f);
            Rect searchRect = new Rect(totalRect);

            _searchField.SetFocus();
            _search = _searchField.OnGUI(searchRect, _search);
        }

        void DrawTree(List<string> list)
        {
            GUIContent[] names = list
                    .Select(x => new GUIContent(x))
                    .ToArray();

            int newIndex = GUILayout.SelectionGrid(_index, names, 1, Styles.ListItemStyle);

            if (newIndex != -1)
                _index = newIndex;
        }

        static class Styles
        {
            public static GUIStyle ListItemStyle => new GUIStyle(EditorStyles.toolbarButton) { alignment = TextAnchor.MiddleLeft, };
        }
    }
}