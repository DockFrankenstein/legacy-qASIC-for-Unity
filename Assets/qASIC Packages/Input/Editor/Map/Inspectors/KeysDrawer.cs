using qASIC.EditorTools;
using qASIC.Input.Internal.KeyProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using static qASIC.EditorTools.EditorChangeChecker;

namespace qASIC.Input.Map.Internal
{
    public class KeysDrawer
    {
        List<string> _keys;

        ReorderableList _keyPathReorderableList;

        List<KeyTypeList> _keyLists = new List<KeyTypeList>();

        int _keyViewMode;

        public event Action OnDirty;
        public bool PathsHaveErrors { get; set; }

        public void Initialize(List<string> keys)
        {
            _keys = keys;

            //Create key lists for types
            _keyLists = InputMapUtility.KeyTypeProviders
                .Select(GenerateList)
                .ToList();

            _keyPathReorderableList = new ReorderableList(_keys, typeof(string), true, true, true, true);
            _keyPathReorderableList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Key paths");
            _keyPathReorderableList.onAddCallback += _ =>
            {
                _keys.Add(string.Empty);
            };

            _keyPathReorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                string rootPath = _keys[index].Split('/').FirstOrDefault();
                if (InputMapUtility.KeyTypeProviders.Where(x => x.RootPath == rootPath).Count() == 0)
                {
                    Rect errorRect = rect.SetWidth(rect.height);
                    rect = rect.BorderLeft(rect.height + 2f);
                    GUI.Label(errorRect, qGUIEditorUtility.ErrorIcon);
                }

                _keys[index] = EditorGUI.DelayedTextField(rect, _keys[index]);
            };
        }

        KeyTypeList GenerateList(KeyTypeProvider provider)
        {
            string keyName = $"{provider.RootPath}/";

            List<int> indexes = new List<int>();
            List<string> keys = _keys
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
            _keys[index] = $"{list.provider.RootPath}/{path}";
            OnDirty?.Invoke();
        }
        #endregion

        #region Reorderable Lists callbacks
        void ReorderableList_OnAddCallback(KeyTypeList list, ReorderableList reorderableList)
        {
            string item = $"{list.provider.RootPath}/";
            _keys.Add(item);
            list.indexes.Add(_keys.Count - 1);
            list.keys.Add(item);
            OnDirty?.Invoke();
        }

        void ReorderableList_OnRemoveCallback(KeyTypeList list, ReorderableList reorderableList)
        {
            int index = reorderableList.index;
            _keys.RemoveAt(list.indexes[index]);
            list.keys.RemoveAt(index);
            list.indexes.RemoveAt(index);
        }

        void ReorderableList_OnReorderCallbackWithDetails(KeyTypeList list, ReorderableList reorderableList, int oldIndex, int newIndex)
        {
            int trueOldIndex = list.indexes[oldIndex];
            int trueNewIndex = list.indexes[newIndex];
            string temp = _keys[trueNewIndex];
            _keys[trueNewIndex] = _keys[trueOldIndex];
            _keys[trueOldIndex] = temp;
            OnDirty?.Invoke();
        }
        #endregion

        public void OnGUI()
        {
            EditorGUILayout.Space();

            GUIContent[] toolbarContent = new GUIContent[]
            {
                new GUIContent("Keys"),
                new GUIContent("Raw", PathsHaveErrors ? qGUIEditorUtility.ErrorIcon : null),
            };

            using (new ChangeCheckPause())
                _keyViewMode = GUILayout.Toolbar(_keyViewMode, toolbarContent, GUILayout.Height(20f));

            EditorGUILayout.Space();

            using (new GUILayout.VerticalScope(Styles.ListSpace))
            {
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
                PopupWindow.Show(rect, new InputBindingSearchPopupContent(provider, a => Popup_OnApply(index, a), new Vector2(rect.width, 200f)));
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

        static class Styles
        {
            public static GUIStyle ListSpace => new GUIStyle()
            {
                margin = new RectOffset(4, 4, 0, 0),
            };
        }
    }

    public class InputBindingSearchPopupContent : PopupWindowContent
    {
        public InputBindingSearchPopupContent(KeyTypeProvider provider, Action<string> onApply, Vector2 size) : this(provider.RootPath, onApply, size) { }
        public InputBindingSearchPopupContent(string rootPath, Action<string> onApply, Vector2 size) : base()
        {
            _size = size;
            _OnApply = onApply;

            if (rootPath.EndsWith("/"))
                rootPath = rootPath.Remove(rootPath.Length - 1, 1);

            _rootPath = rootPath;
        }

        Vector2 _size;
        string _rootPath;

        Vector2 _scroll;
        string _search = string.Empty;
        int _index = 0;

        public override Vector2 GetWindowSize() =>
            _size;

        List<string> _items;
        List<string> _currentItems;
        Action<string> _OnApply;
        bool _keyDown;

        SearchField _searchField = new SearchField();

        public override void OnOpen()
        {
            base.OnOpen();

            _items = InputMapUtility.KeyList
                .Where(x => string.IsNullOrEmpty(_rootPath) || x.Split('/').First() == _rootPath)
                .Select(x => x.Split('/').Last())
                .ToList();

            _currentItems = _items;
        }

        public override void OnGUI(Rect rect)
        {
            DrawTopBar();
            EditorGUILayout.Space(8f);
            qGUIEditorUtility.HorizontalLineLayout();

            _currentItems = qGUIEditorUtility.SortSearchList(_items, _search)
                .ToList();

            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scroll))
            {
                _scroll = scrollView.scrollPosition;
                DrawTree(_currentItems);
            }

            qGUIEditorUtility.HorizontalLineLayout();
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
            _OnApply?.Invoke(_index == -1 ? string.Empty : _currentItems[_index]);
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