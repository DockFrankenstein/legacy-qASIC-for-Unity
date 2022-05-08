#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;
using qASIC.FileManagement;
using qASIC.EditorTools.Internal;
using UnityEditor.ShortcutManagement;

//FIXME: For some reason when recompiling Unity flashes when this window is oppened
namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindow : EditorWindow, IHasCustomMenu
    {
        [SerializeField] Texture2D icon;

        static InputMap _map;
        public static InputMap Map
        {
            get => _map;
            set
            {
                SaveUnmodifiedMap(value);
                _map = value;
            }
        }

        InputMapWindowToolbar toolbar = new InputMapWindowToolbar();
        InputMapWindowGroupBar groupBar = new InputMapWindowGroupBar();
        InputMapWindowInspector inspector = new InputMapWindowInspector();

        InputMapWindowContentTree contentTree;
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
                EditorPrefs.SetBool(prefsKey_autoSave, value);

                if (IsDirty)
                    Save();

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
                EditorPrefs.SetBool(prefsKey_showItemIcons, value);
                prefs_showItemIcons = value;
            }
        }

        public static void ResetPreferences()
        {
            EditorPrefs.DeleteKey(prefsKey_autoSave);
            EditorPrefs.DeleteKey(prefskey_autoSaveTimeLimit);
            EditorPrefs.DeleteKey(prefsKey_inspectorWidth);
            AdvancedEditorPrefs.DeleteColorKey(prefsKey_defaultGroupColor);

            prefs_autoSave = null;
            prefs_autoSaveTimeLimit = null;
            prefs_inspectorWidth = null;
            prefs_defaultGroupColor = null;
        }
        #endregion

        #region Opening
        static string MapPrefsKey => $"{Application.productName}_qASIC_input_map_editor_map";

        internal static string GetMapPath() =>
            EditorPrefs.GetString(MapPrefsKey, "NULL");

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

        [MenuItem("Window/qASIC/Input Map Editor")]
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
            if (Map && Map != newMap && !window.ConfirmSaveChangesIfNeeded(false))
                return;

            OpenMap(newMap);
        }

        public static void OpenMap(InputMap newMap)
        {
            if (Map != newMap)
            {
                GetEditorWindow().groupBar.SelectedGroupIndex = 0;
                Map = newMap;
                EditorPrefs.SetString(MapPrefsKey, AssetDatabase.GetAssetPath(Map));
            }

            OpenWindow();
        }

        internal void LoadMap()
        {
            if (!EditorPrefs.HasKey(MapPrefsKey)) return;

            string mapPath = GetMapPath();
            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(mapPath))) return;
            Map = (InputMap)AssetDatabase.LoadAssetAtPath(mapPath, typeof(InputMap));
        }

        public static void CloseMap() 
        {
            Cleanup();
            EditorPrefs.DeleteKey(MapPrefsKey);
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
            Save();
        }

        [Shortcut("Cablebox Map Editor/Set Group As Default", typeof(InputMapWindow), KeyCode.G, ShortcutModifiers.Alt)]
        private static void DefaultGroupShortcut(ShortcutArguments args)
        {
            if (!Map) return;
            Map.defaultGroup = GetEditorWindow().groupBar.SelectedGroupIndex;
            SetMapDirty(); 
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
            contentTree?.Reload();
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
            toolbar = new InputMapWindowToolbar();
            groupBar = new InputMapWindowGroupBar();
            inspector = new InputMapWindowInspector();

            //Trees
            InitTree();

            //Assigning maps
            toolbar.map = Map;
            inspector.map = Map;

            //Events
            groupBar.OnItemSelect += (object o) =>
            {
                if (contentTree != null)
                {
                    contentTree.Group = o as InputGroup;
                    contentTree.Reload();
                }

                inspector.SetObject(o);
            };

            contentTree.OnItemSelect += (object o) =>
            {
                inspector.SetObject(o); 
            };

            inspector.OnDeleteGroup += groupBar.DeleteGroup;

            inspector.OnDeleteAction += (InputMapWindowInspector.InspectorInputAction action) =>
            {
                int index = action.group.actions.IndexOf(action.action);
                if (index == -1) return;

                action.group.actions.RemoveAt(index);
                contentTree.Reload();
                SelectInInspector(null);
            };

            inspector.OnDeleteAxis += (InputMapWindowInspector.InspectorInputAxis axis) =>
            {
                int index = axis.group.axes.IndexOf(axis.axis);
                if (index == -1) return;

                axis.group.axes.RemoveAt(index);
                contentTree.Reload();
                SelectInInspector(null);
            };
        }

        void InitTree()
        {
            if (contentTreeState == null)
                contentTreeState = new TreeViewState();

            contentTree = new InputMapWindowContentTree(contentTreeState, Map ? Map.Groups.ElementAtOrDefault(groupBar.SelectedGroupIndex) : null);


            if (contentTree.ActionsRoot != null)
                contentTree.SetExpanded(contentTree.ActionsRoot.id, PlayerPrefs.GetInt(prefsKey_treeActionsExpanded, 1) != 0);

            if (contentTree.ActionsRoot != null)
                contentTree.SetExpanded(contentTree.AxesRoot.id, PlayerPrefs.GetInt(prefsKey_treeAxesExpanded, 1) != 0);


            contentTree.OnExpand += () =>
            {
                PlayerPrefs.SetInt(prefsKey_treeActionsExpanded, contentTree.IsExpanded(contentTree.ActionsRoot.id) ? 1 : 0);
                PlayerPrefs.SetInt(prefsKey_treeAxesExpanded, contentTree.IsExpanded(contentTree.AxesRoot.id) ? 1 : 0);
            };
        }

        public static void Cleanup(bool resetMap = true)
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
            groupBar.SetMap(Map);

            toolbar.OnGUI();
            groupBar.OnGUI();

            GUILayout.BeginHorizontal();

            DrawTreeView(contentTree);

            HorizontalLine();

            GUILayout.BeginVertical(GUILayout.Width(Mathf.Min(Prefs_InspectorWidth, position.width * 0.8f)));
            inspector.OnGUI();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

#if !ENABLE_LEGACY_INPUT_MANAGER
            EditorGUILayout.HelpBox("qASIC input doesn't support the New Input System. Please go to Edit/Project Settings/Player and change Active Input Handling to Input Manager or Both.", MessageType.Warning);
            if (GUILayout.Button("Open Project Settings"))
                SettingsService.OpenProjectSettings("Project/Player");
            EditorGUILayout.Space(16f);
#endif

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
            tree.OnGUI(rect);
        }

        void HorizontalLine()
        {
            GUIStyle style = new GUIStyle()
            {
                fixedWidth = 1f,
                stretchHeight = true,
            };

            style.normal.background = qGUIEditorUtility.BorderTexture;
            GUILayout.Box(GUIContent.none, style);

            //TODO: add inspector resizing

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

        private static bool? _isDirty = null;
        public static bool IsDirty
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

        public static void SetMapDirty()
        {
            _isDirty = true;
            EditorUtility.SetDirty(Map);
            GetEditorWindow().SetWindowTitle();

            if (!Prefs_AutoSave) return;

            if (!CanAutoSave())
            {
                _waitForAutoSave = true;
                return;
            }

            Save();
        }

        public static void ResetMapDirty()
        {
            _isDirty = false;
            _waitForAutoSave = false;
            EditorUtility.ClearDirty(Map);
            GetEditorWindow().SetWindowTitle();
        }

        private static void SaveUnmodifiedMap(InputMap map) =>
            FileManager.SaveFileJSON(GetUnmodifiedMapLocation(), map, true);

        public static void Save()
        {
            _lastSaveTime = (float)EditorApplication.timeSinceStartup;
            _isDirty = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GetEditorWindow().SetWindowTitle();
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
            if (inspector == null) return;
            inspector.SetObject(obj);
        }
        #endregion
    }
}
#endif