#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;
using qASIC.EditorTools.Internal;
using UnityEditor.ShortcutManagement;
using qASIC.Files;

//FIXME: For some reason when recompiling Unity flashes when this window is oppened
namespace qASIC.Input.Map.Internal
{
    public class InputMapWindow : EditorWindow, IHasCustomMenu
    {
        [SerializeField] Texture2D icon;

        InputMap _map;
        public InputMap Map
        {
            get => _map;
            set
            {
                SaveUnmodifiedMap(value);
                _map = value;
            }
        }

        public int SelectedGroup =>
            _groupBar.SelectedGroupIndex;

        InputMapWindowToolbar _toolbar = new InputMapWindowToolbar();
        InputMapWindowGroupBar _groupBar = new InputMapWindowGroupBar();
        InputMapWindowInspector _inspector = new InputMapWindowInspector();

        InputMapWindowContentTree _contentTree;
        TreeViewState contentTreeState;

        #region Preferences
        const string prefsKey_autoSave = "qASIC_input_map_editor_autosave";
        const string prefskey_autoSaveTimeLimit = "qASIC_input_map_editor_autosave_timelimit";
        const string prefsKey_debug = "qASIC_input_map_editor_debug";

        const string prefsKey_treeActionsExpanded = "qASIC_input_map_editor_actions_expanded";
        const string prefsKey_treeAxesExpanded = "qASIC_input_map_editor_axes_expanded";

        const string prefsKey_inspectorWidth = "qASIC_input_map_editor_inspector_width";
        const string prefsKey_defaultGroupColor = "qASIC_input_map_editor_defaultgroup_color";
        const string prefsKey_showItemIcons = "qASIC_input_map_editor_showitemicons";
        const string prefsKey_defaultBindingKeys = "qASIC_input_map_editor_default_binding_keys";


        private static bool? _debugMode = null;
        public static bool DebugMode
        {
            get
            {
                if (_debugMode == null)
                    _debugMode = EditorPrefs.GetBool(prefsKey_debug, false);
                return _debugMode ?? false;
            }
            set
            {
                if (value == _debugMode)
                    return;

                EditorPrefs.SetBool(prefsKey_debug, value);
                _debugMode = value;
            }
        }

        private static bool? prefs_autoSave = null;
        public static bool Prefs_AutoSave
        {
            get
            {
                if (prefs_autoSave == null)
                    prefs_autoSave = EditorPrefs.GetBool(prefsKey_autoSave, true);

                return prefs_autoSave ?? false;
            }
            set
            {
                if (value == Prefs_AutoSave)
                    return;

                EditorPrefs.SetBool(prefsKey_autoSave, value);

                var window = GetEditorWindow();
                if (window.IsDirty)
                    window.Save();

                _lastSaveTime = (float)EditorApplication.timeSinceStartup;
                _waitForAutoSave = false;

                prefs_autoSave = value;
            }
        }

        private static float? prefs_inspectorWidth = null;
        public static float Prefs_InspectorWidth
        {
            get
            {
                if (prefs_inspectorWidth == null)
                    prefs_inspectorWidth = EditorPrefs.GetFloat(prefsKey_inspectorWidth, 300f);

                return Mathf.Max(prefs_inspectorWidth ?? 300f, 220f);
            }
            set
            {
                if (value == prefs_inspectorWidth)
                    return;

                EditorPrefs.SetFloat(prefsKey_inspectorWidth, value);
                prefs_inspectorWidth = value;
            }
        }

        private static float? prefs_autoSaveTimeLimit = null;
        public static float Prefs_AutoSaveTimeLimit
        {
            get
            {
                if (prefs_autoSaveTimeLimit == null)
                    prefs_autoSaveTimeLimit = EditorPrefs.GetFloat(prefskey_autoSaveTimeLimit, 0f);

                return prefs_autoSaveTimeLimit ?? 0f;
            }
            set
            {
                if (value == prefs_autoSaveTimeLimit)
                    return;

                value = Mathf.Max(0f, value);
                EditorPrefs.SetFloat(prefskey_autoSaveTimeLimit, value);
                prefs_autoSaveTimeLimit = value;
            }
        }

        private static Color? prefs_defaultGroupColor;
        public static Color Prefs_DefaultGroupColor
        {
            get
            {
                if (prefs_defaultGroupColor == null)
                    prefs_defaultGroupColor = AdvancedEditorPrefs.GetColor(prefsKey_defaultGroupColor, qGUIInternalUtility.qASICColor);

                return prefs_defaultGroupColor ?? Color.white;
            }
            set
            {
                if (value == prefs_defaultGroupColor)
                    return;

                AdvancedEditorPrefs.SetColorWithAlpha(prefsKey_defaultGroupColor, value);
                prefs_defaultGroupColor = value;
            }
        }

        private static bool? prefs_showItemIcons = null;
        public static bool Prefs_ShowItemIcons
        {
            get
            {
                if (prefs_showItemIcons == null)
                    prefs_showItemIcons = EditorPrefs.GetBool(prefsKey_showItemIcons, true);

                return prefs_showItemIcons ?? false;
            }
            set
            {
                if (value == prefs_showItemIcons)
                    return;

                EditorPrefs.SetBool(prefsKey_showItemIcons, value);
                prefs_showItemIcons = value;
            }
        }

        private static List<string> prefs_defaultBindingKeys = null;
        public static List<string> Prefs_DefaultBindingKeys
        {
            get
            {
                if (prefs_defaultBindingKeys == null)
                {
                    prefs_defaultBindingKeys = new List<string>(EditorPrefs.GetString(prefsKey_defaultBindingKeys, "key_keyboard/\nkey_gamepad/\n").Split('\n'));
                    prefs_defaultBindingKeys.RemoveAt(prefs_defaultBindingKeys.Count - 1);
                }

                return prefs_defaultBindingKeys ?? new List<string>();
            }
            set
            {
                if (value == prefs_defaultBindingKeys)
                    return;

                EditorPrefs.SetString(prefsKey_defaultBindingKeys, $"{string.Join("\n", value)}\n");
                prefs_defaultBindingKeys = value;
            }
        }

        public static void ResetPreferences()
        {
            EditorPrefs.DeleteKey(prefsKey_autoSave);
            EditorPrefs.DeleteKey(prefskey_autoSaveTimeLimit);
            EditorPrefs.DeleteKey(prefsKey_inspectorWidth);
            AdvancedEditorPrefs.DeleteColorKey(prefsKey_defaultGroupColor);
            EditorPrefs.DeleteKey(prefsKey_defaultBindingKeys);

            prefs_autoSave = null;
            prefs_autoSaveTimeLimit = null;
            prefs_inspectorWidth = null;
            prefs_defaultGroupColor = null;
            prefs_defaultBindingKeys = null;
        }
        #endregion

        #region Opening
        static string _MapPrefsKey => $"{Application.productName}_qASIC_input_map_editor_map";

        internal static string GetMapPath() =>
            EditorPrefs.GetString(_MapPrefsKey, "NULL");

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is InputMap map))
                return false;
            OpenMapIfNotDirty(map);
            return true;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem("Debug", DebugMode, () =>
            {
                DebugMode = !DebugMode;
                //Redraw inspector in case any map is selected
                AssetDatabase.Refresh();
            });
        }

        [MenuItem("Window/qASIC/Input/Map Editor")]
        public static InputMapWindow OpenWindow()
        {
            InputMapWindow window = SetupWindowForOpen();
            window.ResetEditor();
            window.Show();
            return window;
        }

        static InputMapWindow SetupWindowForOpen()
        {
            InputMapWindow window = GetEditorWindow();
            window.minSize = new Vector2(512f, 256f);
            window.SetWindowTitle();
            return window;
        }

        public static void OpenMapIfNotDirty(InputMap newMap)
        {
            InputMapWindow window = GetEditorWindow();
            if (window.Map && window.Map != newMap && !window.ConfirmSaveChangesIfNeeded(false))
                return;

            OpenMap(newMap);
        }

        public static void OpenMap(InputMap newMap)
        {
            var window = GetEditorWindow();
            if (window.Map != newMap)
            {
                window._groupBar.SelectedGroupIndex = 0;
                window.Map = newMap;
                EditorPrefs.SetString(_MapPrefsKey, AssetDatabase.GetAssetPath(window.Map));
            }

            OpenWindow();
        }

        internal void LoadMap()
        {
            if (!EditorPrefs.HasKey(_MapPrefsKey)) return;

            string mapPath = GetMapPath();
            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(mapPath))) return;
            Map = (InputMap)AssetDatabase.LoadAssetAtPath(mapPath, typeof(InputMap));
        }
        
        public void CloseMap() 
        {
            Cleanup();
            EditorPrefs.DeleteKey(_MapPrefsKey);
            GetEditorWindow().ResetEditor();
        }

        public static InputMapWindow GetEditorWindow() =>
            (InputMapWindow)GetWindow(typeof(InputMapWindow), false, "Input Map Editor");

        public void SetWindowTitle()
        {
            titleContent = new GUIContent($"{(IsDirty ? "*" : "")}{(Map ? Map.name : "Input Map Editor")}", icon);
        }
        #endregion

        #region Shortcuts
        [Shortcut("Cablebox Map Editor/Save", typeof(InputMapWindow), KeyCode.S, ShortcutModifiers.Alt)]
        private static void SaveShortcut(ShortcutArguments args)
        {
            GetEditorWindow().Save();
        }

        [Shortcut("Cablebox Map Editor/Set Group As Default", typeof(InputMapWindow), KeyCode.G, ShortcutModifiers.Alt)]
        private static void DefaultGroupShortcut(ShortcutArguments args)
        {
            InputMapWindow window = GetEditorWindow();
            if (window == null) return;

            window.Map.defaultGroup = window._groupBar.SelectedGroupIndex;
            window.SetMapDirty(); 
        }
        #endregion

        #region Reset&Destroy
        private void OnEnable()
        {
            EditorApplication.wantsToQuit += OnEditorWantsToQuit;

            if (Map == null)
                LoadMap();

            ResetEditor();
        }

        private bool OnEditorWantsToQuit()
        {
            return ConfirmSaveChangesIfNeeded();
        }

        private void OnDestroy()
        {
            EditorApplication.wantsToQuit -= OnEditorWantsToQuit;
            ConfirmSaveChangesIfNeeded();
        }

        public void ReloadTrees()
        {
            _contentTree?.Reload();
        }

        bool _reloadTreesNextRepaint = false;

        public void ReloadTreesNextRepaint() =>
            _reloadTreesNextRepaint = true;
        
        public void ResetEditor()
        {
            //Map
            _isDirty = Map && EditorUtility.GetDirtyCount(Map.GetInstanceID()) != 0;

            if (Map && !IsDirty)
            {
                SaveUnmodifiedMap(Map);
            }

            //Title
            SetWindowTitle();

            //Displayers
            _toolbar = new InputMapWindowToolbar();
            _groupBar = new InputMapWindowGroupBar();
            _inspector = new InputMapWindowInspector();
            _toolbar.window = this;
            _groupBar.window = this;
            _inspector.window = this;

            //Trees
            InitTree();

            _toolbar.groupBar = _groupBar;
            _toolbar.inspector = _inspector;
            _toolbar.contentTree = _contentTree;

            //Assigning maps
            _toolbar.map = Map;
            _inspector.map = Map;

            //Events
            _groupBar.OnItemSelect += (g) =>
            {
                if (_contentTree != null)
                {
                    _contentTree.Group = g;
                    _contentTree.Reload();
                }

                _inspector.SetObject(g);
            };
        }

        void InitTree()
        {
            if (contentTreeState == null)
                contentTreeState = new TreeViewState();

            _contentTree = new InputMapWindowContentTree(contentTreeState, Map ? Map.groups.ElementAtOrDefault(_groupBar.SelectedGroupIndex) : null);
            _contentTree.window = this;

            if (_contentTree.BindingsRoot != null)
                _contentTree.SetExpanded(_contentTree.BindingsRoot.id, PlayerPrefs.GetInt(prefsKey_treeActionsExpanded, 1) != 0);

            if (_contentTree.BindingsRoot != null)
                _contentTree.SetExpanded(_contentTree.OthersRoot.id, PlayerPrefs.GetInt(prefsKey_treeAxesExpanded, 1) != 0);


            _contentTree.OnExpand += () =>
            {
                PlayerPrefs.SetInt(prefsKey_treeActionsExpanded, _contentTree.IsExpanded(_contentTree.BindingsRoot.id) ? 1 : 0);
                PlayerPrefs.SetInt(prefsKey_treeAxesExpanded, _contentTree.IsExpanded(_contentTree.OthersRoot.id) ? 1 : 0);
            };
        }

        public void Cleanup(bool resetMap = true)
        {
            if (resetMap)
                Map = null;

            _isDirty = false;
            DeleteUnmodified();
        }

        public static void DeleteUnmodified()
        {
            if (FileManager.FileExists(GetUnmodifiedMapLocation()))
                FileManager.DeleteFile(GetUnmodifiedMapLocation());
        }
        #endregion

        #region GUI
        bool _resizingInspector;
        float _inspectorLineCursorOffset; 
         
        private void OnGUI()
        {
            if (Map?.Initialized == false)
                Map.Initialize();

            _groupBar.SetMap(Map);

            _toolbar.OnGUI();
            _groupBar.OnGUI();

            GUILayout.BeginHorizontal();

            DrawTreeView(_contentTree);
            
            HorizontalLine();

            float width = Mathf.Min(Prefs_InspectorWidth, position.width * 0.8f);
            GUILayout.BeginVertical(GUILayout.Width(width));
            using (new GUILayout.VerticalScope())
            {
                _inspector.Width = width;
                _inspector.OnGUI();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (Event.current.type != EventType.Repaint) return;

            if (_reloadTreesNextRepaint)
            {
                _reloadTreesNextRepaint = false;
                ReloadTrees();
            }
        }

        private void Update()
        {
            if (_waitForAutoSave && CanAutoSave())
            {
                _waitForAutoSave = false;
                Save();
            }
        }

        void DrawTreeView(TreeView tree)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            Rect rect = GUILayoutUtility.GetLastRect();
            tree?.OnGUI(rect);
        }

        void HorizontalLine()
        {
            qGUIEditorUtility.VerticalLineLayout();

            Rect lineRect = GUILayoutUtility.GetLastRect();
            Rect mouseAreaRect = new Rect(lineRect).Border(-2f, 0f);

            EditorGUIUtility.AddCursorRect(mouseAreaRect, MouseCursor.ResizeHorizontal);

            if (Event.current.rawType == EventType.MouseDown &&
                mouseAreaRect.Contains(Event.current.mousePosition))
            {
                _inspectorLineCursorOffset = lineRect.x - Event.current.mousePosition.x;
                _resizingInspector = true;
            }

            if (_resizingInspector)
            {
                Prefs_InspectorWidth = position.width - Event.current.mousePosition.x - _inspectorLineCursorOffset - lineRect.width;
                Repaint();
            }

            if (Event.current.rawType == EventType.MouseUp)
            {
                _inspectorLineCursorOffset = 0f;
                _resizingInspector = false;
            }
        }
        #endregion

        #region Saving
        static bool _waitForAutoSave = false;

        public static string GetUnmodifiedMapLocation() =>
            $"{Application.persistentDataPath}/qASIC_inputmap-unmodified.txt";

        private bool? _isDirty = null;
        public bool IsDirty
        {
            get
            {
                if (!Map)
                    return false;

                if (_isDirty == null)
                    _isDirty = EditorUtility.GetDirtyCount(Map) != 0;

                return _isDirty ?? false;
            }
        }

        public static double TimeToAutoSave => _lastSaveTime + Prefs_AutoSaveTimeLimit - EditorApplication.timeSinceStartup;

        private static float _lastSaveTime = float.NegativeInfinity;

        public static bool CanAutoSave() =>
            _lastSaveTime + Prefs_AutoSaveTimeLimit < EditorApplication.timeSinceStartup;

        public static float TimeSinceLastSave =>
            (float)EditorApplication.timeSinceStartup - _lastSaveTime;

        public void SetMapDirty()
        {
            _isDirty = true;
            EditorUtility.SetDirty(Map);
            SetWindowTitle();

            if (!Prefs_AutoSave) return;

            if (!CanAutoSave())
            {
                _waitForAutoSave = true;
                return;
            }

            Save();
        }

        public void ResetMapDirty()
        {
            _isDirty = false;
            _waitForAutoSave = false;
            EditorUtility.ClearDirty(Map);
            SetWindowTitle();
        }

        private void SaveUnmodifiedMap(InputMap map) =>
            FileManager.SaveFileJSON(GetUnmodifiedMapLocation(), map, true);

        public void Save()
        {
            _lastSaveTime = (float)EditorApplication.timeSinceStartup;
            _waitForAutoSave = false;
            _isDirty = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SetWindowTitle();
            SaveUnmodifiedMap(Map);
        }

        public void DiscardMapChanges()
        {
            _isDirty = false;
            if (FileManager.TryReadFileJSON(GetUnmodifiedMapLocation(), Map))
            {
                AssetDatabase.SaveAssets();
                EditorUtility.ClearDirty(Map);
            }

            SetWindowTitle();
            ReloadTrees();
            InputMap map = Map;
            Cleanup();
            OpenMap(map);
        }

        public bool ConfirmSaveChangesIfNeeded(bool reoppenOnCancel = true, bool deleteUnmodified = true)
        {
            if (!Map || !IsDirty)
            {
                if (deleteUnmodified)
                    DeleteUnmodified();
                return true;
            }

            int result = EditorUtility.DisplayDialogComplex("Input Map has been modified",
                $"Would you like to save changes you made to '{Map.name}'",
                "Save", "Discard changes", "Cancel");
            switch(result)
            {
                case 0:
                    //Save
                    Save();
                    return true;
                case 1:
                    //Discard changes
                    DiscardMapChanges();
                    if (deleteUnmodified)
                        DeleteUnmodified();
                    return true;
                default:
                    //Cancel
                    if (reoppenOnCancel)
                        Instantiate(this).Show();
                    return false;
            }
        }
        #endregion

        #region Other
        public void SelectInInspector(object obj)
        {
            if (_inspector == null) return;
            _inspector.SetObject(obj);
        }

        public void SetInspector(Inspectors.InputMapItemInspector inspector)
        {
            _inspector?.SetInspector(inspector);
        }

        public void AddItem<T>() where T : InputMapItem =>
           _contentTree.AddItem<T>();

        public void AddItem(System.Type type) =>
            _contentTree.AddItem(type);

        public void AddItem(InputMapItem item) =>
            _contentTree.AddItem(item);

        public void SetAsDefaultProjectMapDialogue()
        {
            if (!EditorUtility.DisplayDialog("Confirm map assignment", "Are you sure you want to set this as the default project map?", "Yes", "No"))
                return;

            ProjectSettings.InputProjectSettings.Instance.map = Map;
        }
        #endregion
    }
}
#endif