﻿using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;
using qASIC.FileManagement;

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
                FileManager.SaveFileJSON(GetUnmodifiedMapLocation(), value);
                _map = value;
            }
        }

        InputMapWindowToolbar toolbar = new InputMapWindowToolbar();
        InputMapWindowGroupBar groupBar = new InputMapWindowGroupBar();
        InputMapWindowInspector inspector = new InputMapWindowInspector();

        InputMapWindowContentTree contentTree;
        TreeViewState contentTreeState;

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

        #region Preferences
        const string autoSavePrefsKey = "qASIC_input_map_editor_autosave";
        const string debugPrefsKey = "qASIC_input_map_editor_debug";

        static bool? _debugMode = null;
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

        static bool? _autoSave = null;
        public static bool AutoSave {
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

        public static void ResetPreferences()
        {
            EditorPrefs.DeleteKey(autoSavePrefsKey);
            EditorPrefs.DeleteKey(debugPrefsKey);

            _autoSave = null;
            _debugMode = null;
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
            if (Map && Map != newMap)
                GetEditorWindow().groupBar.SelectedGroupIndex = 0;

            Map = newMap;
            EditorPrefs.SetString(MapPrefsKey, AssetDatabase.GetAssetPath(Map));
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
            if (contentTreeState == null)
                contentTreeState = new TreeViewState();

            contentTree = new InputMapWindowContentTree(contentTreeState, Map ? Map.Groups.ElementAtOrDefault(groupBar.SelectedGroupIndex) : null);

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
            };
        }

        public static void Cleanup()
        {
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

            GUILayout.BeginVertical(GUILayout.Width(300f));
            inspector.OnGUI();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

#if !ENABLE_LEGACY_INPUT_MANAGER
            EditorGUILayout.HelpBox("qASIC input doesn't support the New Input System. Please go to Edit/Project Settings/Player and change Active Input Handling to Input Manager or Both.", MessageType.Warning);
            if (GUILayout.Button("Open Project Settings"))
                SettingsService.OpenProjectSettings("Project/Player");
            EditorGUILayout.Space(16f);
#endif
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

            style.normal.background = qGUIUtility.BorderTexture;
            GUILayout.Box(GUIContent.none, style);
        }
        #endregion

        #region Saving
        public static void SetMapDirty()
        {
            _isDirty = true;
            EditorUtility.SetDirty(Map);
            GetEditorWindow().SetWindowTitle();
            
            if (AutoSave)
                Save();
        }

        public static void Save()
        {
            _isDirty = false;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GetEditorWindow().SetWindowTitle();
        }

        public void DiscardChanges()
        {
            _isDirty = false;
            if (FileManager.TryReadFileJSON(GetUnmodifiedMapLocation(), Map))
            {
                AssetDatabase.SaveAssets();
                EditorUtility.ClearDirty(Map);
            }

            SetWindowTitle();
            ReloadTrees();
            Cleanup();
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
                    DiscardChanges();
                    return true;
                default:
                    //Cancel
                    if (reoppenOnCancel)
                        Instantiate(this).Show();
                    return false;
            }
        }
        #endregion
    }
}