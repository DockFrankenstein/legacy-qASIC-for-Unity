using UnityEditor;
using UnityEngine;
using qASIC.Input.Devices;
using qASIC.EditorTools;
using System.Linq;
using System.Collections;

using UInput = UnityEngine.Input;
using qASIC;

namespace qASIC.Input.DebugTools
{
    public class InputDebug : EditorWindow
    {
        [MenuItem("Window/qASIC/Input/Input Debug")]
        public static InputDebug OpenWindow()
        {
            InputDebug window = GetEditorWindow();
            window.minSize = new Vector2(300f, 300f);
            window.Show();
            return window;
        }

        public static InputDebug GetEditorWindow() =>
            (InputDebug)GetWindow(typeof(InputDebug), false, "Input Debug");

        int? _page = null;
        int Page
        {
            get
            {
                if (_page == null)
                    _page = SessionState.GetInt("cablebox_input_debug_page", 0);

                return _page ?? 0;
            }
            set
            {
                SessionState.SetInt("cablebox_input_debug_page", value);
                _page = value;
            }
        }

        Vector2 _menuSelectionScroll;
        Vector2 _contentScroll;

        int _mapDataSelectedPlayer;

        string _keySearch;

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (var scroll = new GUILayout.ScrollViewScope(_menuSelectionScroll, GUILayout.Width(150f)))
                {
                    _menuSelectionScroll = scroll.scrollPosition;
                    MenuSelectionGUI();
                }

                qGUIEditorUtility.VerticalLineLayout();

                using (var scroll = new GUILayout.ScrollViewScope(_contentScroll))
                {
                    _contentScroll = scroll.scrollPosition;
                    ContentGUI();
                }
            }
        }

        void MenuSelectionGUI()
        {
            Page = GUILayout.SelectionGrid(Page, new string[] { "Utility", "UIM", "Map Data", "Key List", "Cursor" }, 1, Styles.MenuSelection);
        }

        void ContentGUI()
        {
            switch (Page)
            {
                case 0:
                    if (GUILayout.Button("Reload Device Manager"))
                        DeviceManager.Reload();
                    break;
                case 1:
                    string[] joystickNames = UInput.GetJoystickNames();

                    TextTree uimJoysticksTree = new TextTree(TextTreeStyle.Fancy);
                    TextTreeItem uimJoysticksRoot = new TextTreeItem($"UIM Joysticks (count: {joystickNames.Length})");

                    for (int i = 0; i < joystickNames.Length; i++)
                        uimJoysticksRoot.Add(string.IsNullOrEmpty(joystickNames[i]) ? "[DISCONNECTED]" : joystickNames[i]);

                    GUILayout.Label(uimJoysticksTree.GenerateTree(uimJoysticksRoot), Styles.TextTree);
                    break;
                case 2:
                    if (!Application.isPlaying)
                    {
                        EditorGUILayout.HelpBox("Input Map Data is only generated during runtime", MessageType.Info);
                        break;
                    }

                    string[] playerNames = InputManager.Players
                        .Select(x => x.ID)
                        .ToArray();

                    _mapDataSelectedPlayer = EditorGUILayout.Popup("Selected Player", _mapDataSelectedPlayer, playerNames);

                    TextTree mapDataTree = new TextTree(TextTreeStyle.Fancy);
                    TextTreeItem mapDataTreeRoot = new TextTreeItem("Map Data");

                    var player = InputManager.Players[_mapDataSelectedPlayer];
                    var map = player.Map;

                    EditorGUILayout.Space();

                    foreach (var item in player.MapData)
                    {                   
                        var mapItem = map.GetItem<Map.InputMapItem>(item.Key);
                        var itemGroup = map.groups
                            .Where(x => x.items.Contains(mapItem))
                            .First();

                        TextTreeItem mapTreeItem = new TextTreeItem($"{itemGroup.ItemName}/{mapItem.ItemName}");

                        CreateFieldTextTree(item.Value, mapTreeItem);

                        mapDataTreeRoot.Add(mapTreeItem);
                    }

                    GUILayout.Label(mapDataTree.GenerateTree(mapDataTreeRoot), Styles.TextTree);

                    EditorGUILayout.Space();

                    switch (GUILayout.SelectionGrid(-1, new string[] { "Load", "Save", "Reset Data" }, 3))
                    {
                        case 0:
                            player.Load();
                            break;
                        case 1:
                            player.Save();
                            break;
                        case 2:
                            player.ResetData();
                            break;
                    }
                    break;
                case 3:
                    _keySearch = EditorGUILayout.TextField(_keySearch, Styles.SearchBar);

                    var items = qGUIEditorUtility.SortSearchList(Map.InputMapUtility.KeyList, _keySearch)
                        .ToList();

                    TextTree keyTextTree = new TextTree(TextTreeStyle.Fancy);
                    TextTreeItem keyTextTreeRootItem = new TextTreeItem("Keys");

                    foreach (var key in items)
                        keyTextTreeRootItem.Add(key);

                    GUILayout.Label(keyTextTree.GenerateTree(keyTextTreeRootItem), Styles.TextTree);
                    break;
                case 4:
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Unlock cursor")) Cursor.lockState = CursorLockMode.None;
                        if (GUILayout.Button("Lock cursor")) Cursor.lockState = CursorLockMode.Locked;
                        if (GUILayout.Button("Confine")) Cursor.lockState = CursorLockMode.Confined;
                    }
                    break;
            }
        }

        //not my greatest work, but it's good enough
        void CreateFieldTextTree(object obj, TextTreeItem parentTreeItem)
        {
            var fields = obj.GetType()
                .GetFields();

            foreach (var field in fields)
            {
                var value = field.GetValue(obj);
                switch (value)
                {
                    case IList list:
                        TextTreeItem listRoot = new TextTreeItem($"{field.Name} (list)");
                        foreach (var item in list)
                            listRoot.Add(item?.ToString() ?? "NULL");

                        parentTreeItem.Add(listRoot);
                        break;
                    default:
                        switch (field.FieldType.IsValueType)
                        {
                            case true:
                                parentTreeItem.Add($"{field.Name}: {value}");
                                break;
                            case false:
                                TextTreeItem treeItem = new TextTreeItem(field.Name);
                                CreateFieldTextTree(value, treeItem);
                                parentTreeItem.Add(treeItem);
                                break;
                        }
                        break;
                }
            }
        }

        private static class Styles
        {
            public static GUIStyle MenuSelection => new GUIStyle(EditorStyles.toolbarButton)
            {
                fixedHeight = 36,
            };

            public static GUIStyle TextTree => new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 18,
            };

            public static GUIStyle SearchBar => new GUIStyle(EditorStyles.toolbarSearchField)
            {
                margin = new RectOffset(8, 8, 4, 4),
            };
        }
    }
}