using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;
using qASIC.FileManagement;
using System.Collections.Generic;

//FIXME: For some reason when recompiling Unity flashes when this window is oppened
namespace qASIC.InputManagement.Map.Internal
{
    public class InputMapWindow : EditorWindow, IHasCustomMenu
    {
        [SerializeField] Texture2D icon;

        static InputMap _map;
        static InputMap Map
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
        const string autoSavePrefsKey = "qASIC_input_map_editor_autosave";
        const string autoSaveTimeLimitPrefsKey = "qASIC_input_map_editor_autosave_timelimit";
        const string debugPrefsKey = "qASIC_input_map_editor_debug";

        const string treeActionsExpandedPrefsKey = "qASIC_input_map_editor_actions_expanded";
        const string treeAxesExpandedPrefsKey = "qASIC_input_map_editor_axes_expanded";

        const string inspectorWidthPrefsKey = "qASIC_input_map_editor_inspector_width";


        private static bool? _debugMode = null;
        public static bool DebugMode
        {
            get
            {
                if (_debugMode == null)
                    _debugMode = EditorPrefs.GetBool(debugPrefsKey, false);
                return _debugMode ?? false;
            }
            set
            {
                EditorPrefs.SetBool(debugPrefsKey, value);
                _debugMode = value;
            }
        }

        private static bool? _autoSave = null;
        public static bool AutoSave
        {
            get
            {
                if (_autoSave == null)
                    _autoSave = EditorPrefs.GetBool(autoSavePrefsKey, true);

                return _autoSave ?? false;
            }
            set
            {
                EditorPrefs.SetBool(autoSavePrefsKey, value);

                if (IsDirty)
                    Save();

                _autoSave = value;
            }
        }

        private static float? _inspectorWidth = null;
        public static float InspectorWidth
        {
            get
            {
                if (_inspectorWidth == null)
                    _inspectorWidth = EditorPrefs.GetFloat(inspectorWidthPrefsKey, 300f);

                return Mathf.Max(_inspectorWidth ?? 300f, 220f);
            }
            set
            {
                EditorPrefs.SetFloat(inspectorWidthPrefsKey, value);
                _inspectorWidth = value;
            }
        }

        private static float? _autoSaveTimeLimit = null;
        public static float AutoSaveTimeLimit
        {
            get
            {
                if (_autoSaveTimeLimit == null)
                    _autoSaveTimeLimit = EditorPrefs.GetFloat(autoSaveTimeLimitPrefsKey, 0f);

                return _autoSaveTimeLimit ?? 0f;
            }
            set
            {
                value = Mathf.Max(0f, value);
                EditorPrefs.SetFloat(autoSaveTimeLimitPrefsKey, value);
                _autoSaveTimeLimit = value;
            }
        }

        public static void ResetPreferences()
        {
            EditorPrefs.DeleteKey(autoSavePrefsKey);
            EditorPrefs.DeleteKey(debugPrefsKey);
            EditorPrefs.DeleteKey(inspectorWidthPrefsKey);
            EditorPrefs.DeleteKey(autoSaveTimeLimitPrefsKey);

            _autoSave = null;
            _debugMode = null;
            _inspectorWidth = null;
            _autoSaveTimeLimit = null;
        }
        #endregion

        #region Opening
        static string MapPrefsKey => $"{Application.productName}_qASIC_input_map_editor_map";

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

        void LoadMap()
        {
            if (!EditorPrefs.HasKey(MapPrefsKey)) return;

            string mapPath = EditorPrefs.GetString(MapPrefsKey);
            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(mapPath))) return;
            Map = (InputMap)AssetDatabase.LoadAssetAtPath(mapPath, typeof(InputMap));
        }

        public static void CloseMap()
        {
            Map = null;
            EditorPrefs.DeleteKey(MapPrefsKey);
            Cleanup();
            GetEditorWindow().ResetEditor();
        }

        public static InputMapWindow GetEditorWindow() =>
            (InputMapWindow)GetWindow(typeof(InputMapWindow), false, "Input Map Editor");

        public void SetWindowTitle()
        {
            titleContent = new GUIContent($"{(IsDirty ? "*" : "")}{(Map ? Map.name : "Input Map Editor")}", icon);
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
                contentTree.SetExpanded(contentTree.ActionsRoot.id, PlayerPrefs.GetInt(treeActionsExpandedPrefsKey, 1) != 0);

            if (contentTree.ActionsRoot != null)
                contentTree.SetExpanded(contentTree.AxesRoot.id, PlayerPrefs.GetInt(treeAxesExpandedPrefsKey, 1) != 0);


            contentTree.OnExpand += () =>
            {
                PlayerPrefs.SetInt(treeActionsExpandedPrefsKey, contentTree.IsExpanded(contentTree.ActionsRoot.id) ? 1 : 0);
                PlayerPrefs.SetInt(treeAxesExpandedPrefsKey, contentTree.IsExpanded(contentTree.AxesRoot.id) ? 1 : 0);
            };
        }

        public static void Cleanup(bool resetMap = true)
        {
            if (resetMap)
                Map = null;

            _isDirty = false;
            if (FileManager.FileExists(GetUnmodifiedMapLocation()))
                FileManager.DeleteFile(GetUnmodifiedMapLocation());

            EditorPrefs.DeleteKey(MapPrefsKey);
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            groupBar.SetMap(Map);

            toolbar.OnGUI();
            groupBar.OnGUI();

            GUILayout.BeginHorizontal();

            DrawTreeView(contentTree);

            HorizontalLine();

            GUILayout.BeginVertical(GUILayout.Width(Mathf.Min(InspectorWidth, position.width * 0.8f)));
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

            if (DebugMode)
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect().Border(-2f, 0f), MouseCursor.ResizeHorizontal);
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

        private static float _lastSaveTime = float.NegativeInfinity;

        public static bool CanAutoSave() =>
            _lastSaveTime + AutoSaveTimeLimit < EditorApplication.timeSinceStartup;

        public static float TimeSinceLastSave =>
            (float)EditorApplication.timeSinceStartup - _lastSaveTime;

        public static void SetMapDirty()
        {
            _isDirty = true;
            EditorUtility.SetDirty(Map);
            GetEditorWindow().SetWindowTitle();

            if (!AutoSave) return;

            if (!CanAutoSave())
            {
                _waitForAutoSave = true;
                return;
            }

            Save();
        }

        private static void SaveUnmodifiedMap(InputMap map) =>
            FileManager.SaveFileJSON(GetUnmodifiedMapLocation(), map);

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

        public bool ConfirmSaveChangesIfNeeded(bool reoppenOnCancel = true)
        {
            if (!Map || !IsDirty) return true;
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