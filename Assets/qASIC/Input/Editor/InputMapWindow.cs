using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;

namespace qASIC.InputManagement.Internal
{
    public class InputMapWindow : EditorWindow, IHasCustomMenu
    {
        [SerializeField] Texture2D icon;

        static InputMap map;

        InputMapToolbar toolbar = new InputMapToolbar();
        InputMapGroupBar groupBar = new InputMapGroupBar();
        InputMapInspectorDisplayer inspector = new InputMapInspectorDisplayer();

        InputMapContentTree contentTree;
        TreeViewState contentTreeState;

        const string mapPrefsKey = "qASIC_input_map_editor_map";
        const string autoSavePrefsKey = "qASIC_input_map_editor_autosave";

        static bool _isDirty;
        public static bool IsDirty => map && _isDirty;

        static bool _autoSave;
        public static bool AutoSave {
            get => _autoSave;
            set
            {
                EditorPrefs.SetBool(autoSavePrefsKey, value);
                _autoSave = value;
            }
        }

        #region Opening
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            object obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!(obj is InputMap map))
                return false;
            OpenMap(map);
            return true;
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem("Reset editor", false, CloseMap);
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

        static void LoadMap()
        {
            if (!EditorPrefs.HasKey(mapPrefsKey)) return;

            string mapPath = EditorPrefs.GetString(mapPrefsKey);
            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(mapPath))) return;

            map = (InputMap)AssetDatabase.LoadAssetAtPath(mapPath, typeof(InputMap));
        }

        public static void OpenMap(InputMap newMap)
        {
            map = newMap;
            EditorPrefs.SetString(mapPrefsKey, AssetDatabase.GetAssetPath(newMap));
            OpenWindow();
        }

        public static void CloseMap()
        {
            map = null;
            EditorPrefs.DeleteKey(mapPrefsKey);
            GetEditorWindow().ResetEditor();
        }

        public static InputMapWindow GetEditorWindow() =>
            (InputMapWindow)GetWindow(typeof(InputMapWindow), false, "Input Map Editor");

        public void SetWindowTitle()
        {
            titleContent = new GUIContent($"{(_isDirty ? "*" : "")}{(map ? map.name : "Input Map Editor")}", icon);
        }
        #endregion

        #region Reset&Destroy
        private void OnEnable()
        {
            EditorApplication.wantsToQuit += OnEditorWantsToQuit;

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

        public void ResetEditor()
        {
            //Preferences
            _autoSave = EditorPrefs.GetBool(autoSavePrefsKey, true);

            _isDirty = EditorUtility.GetDirtyCount(map.GetInstanceID()) != 0;

            //Title
            SetWindowTitle();

            //Displayers
            toolbar = new InputMapToolbar();
            groupBar = new InputMapGroupBar();
            inspector = new InputMapInspectorDisplayer();

            //Trees
            if (contentTreeState == null)
                contentTreeState = new TreeViewState();

            contentTree = new InputMapContentTree(contentTreeState, map ? map.Groups.ElementAtOrDefault(map.currentEditorSelectedGroup) : null);

            //Assigning maps
            toolbar.map = map;
            groupBar.map = map;
            inspector.map = map;

            //Events
            groupBar.OnItemSelect += (object o) =>
            {
                if (contentTree != null && o is InputGroup group)
                {
                    contentTree.Group = group;
                    contentTree.Reload();
                }

                inspector.SetObject(o);
            };

            contentTree.OnItemSelect += inspector.SetObject;

            inspector.OnDeleteGroup += groupBar.DeleteGroup;

            inspector.OnDeleteAction += (InputMapInspectorDisplayer.InspectorInputAction action) =>
            {
                int index = action.group.actions.IndexOf(action.action);
                if (index == -1) return;

                action.group.actions.RemoveAt(index);
                contentTree.Reload();
            };
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            toolbar.OnGUI();
            groupBar.OnGUI();

            GUILayout.BeginHorizontal();

            DrawTreeView(contentTree);

            HorizontalLine();

            GUILayout.BeginVertical(GUILayout.Width(300f));
            inspector.OnGUI();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
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
            EditorUtility.SetDirty(map);
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

        public bool ConfirmSaveChangesIfNeeded()
        {
            if (!map || !IsDirty) return true;
            int result = EditorUtility.DisplayDialogComplex("Input Map has been modified",
                $"Would you like to save changes you made to '{map.name}'",
                "Save", "Discard changes (not working yet)", "Cancel");
            switch(result)
            {
                case 0:
                    //Save
                    Save();
                    break;
                case 1:
                    //Discard changes
                    //This has not been added yet - in order to allow discarding,
                    //the map needs to be copied and replaced. For some reason there
                    //isn't a simple solution in Unity yet for this
                    break;
                default:
                    //Cancel
                    Instantiate(this).Show();
                    break;
            }

            return false;
        }
        #endregion
    }
}