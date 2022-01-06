using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Callbacks;
using qASIC.EditorTools;

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
        static void OpenWindow()
        {
            InputMapWindow window = GetEdtorWindow();
            window.titleContent.image = window.icon;
            window.minSize = new Vector2(512f, 256f);
            window.ResetEditor();
            window.Show();
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
            GetEdtorWindow().ResetEditor();
        }

        public static InputMapWindow GetEdtorWindow() =>
            (InputMapWindow)GetWindow(typeof(InputMapWindow), false, "Input Map Editor");

        private void OnEnable()
        {
            LoadMap();
            ResetEditor();
        }

        public void ResetEditor()
        {
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

            inspector.OnDeleteGroup += (InputGroup group) =>
            {
                int index = map.Groups.IndexOf(group);
                if (index == -1) return;

                map.Groups.RemoveAt(index);
                if (map.Groups.Count > 0)
                    groupBar.Select(Mathf.Max(0, index - 1));

                groupBar.ResetScroll();
            };

            inspector.OnDeleteAction += (InputMapInspectorDisplayer.InspectorInputAction action) =>
            {
                int index = action.group.actions.IndexOf(action.action);
                if (index == -1) return;

                action.group.actions.RemoveAt(index);
                contentTree.Reload();
            };
        }

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
    }
}