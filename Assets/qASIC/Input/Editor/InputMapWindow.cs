using UnityEditor;
using UnityEngine;
using qASIC.UnityEditor;
using System.Linq;
using UnityEditor.IMGUI.Controls;

namespace qASIC.InputManagement.Internal
{
    public class InputMapWindow : EditorWindow
    {
        [SerializeField] Texture2D icon;

        static InputMap map;

        InputMapToolbar toolbar = new InputMapToolbar();
        InputMapGroupBar groupBar = new InputMapGroupBar();
        InputMapContentDisplayer content = new InputMapContentDisplayer();
        InputMapInspectorDisplayer inspector = new InputMapInspectorDisplayer();

        InputMapContentTree contentTree;
        TreeViewState contentTreeState;

        const string mapPrefsKey = "qASIC_editor_input_map";

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
            groupBar = new InputMapGroupBar();
            content = new InputMapContentDisplayer();
            inspector = new InputMapInspectorDisplayer();
            toolbar = new InputMapToolbar();

            //Trees
            if (contentTreeState == null)
                contentTreeState = new TreeViewState();

            contentTree = new InputMapContentTree(contentTreeState);


            toolbar.map = map;
            groupBar.map = map;
            inspector.map = map;

            groupBar.OnItemSelect += inspector.SetObject;
            content.OnItemSelect += inspector.SetObject;

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
                if (action.group.actions.Count > 0)
                    content.Select(Mathf.Max(0, index - 1));
            };
        }

        private void OnGUI()
        {
            toolbar.OnGUI();
            groupBar.OnGUI();

            GUILayout.BeginHorizontal();


            GUILayout.BeginVertical();
            content.group = map ? map.Groups.ElementAtOrDefault(map.currentEditorSelectedGroup) : null;
            content.OnGUI();

            //This should also be uncomented for treeview
            //GUILayout.FlexibleSpace();

            GUILayout.EndVertical();

            //@Kubikz do your magic
            //Treeview solution, currently not finished
            //Rect rect = GUILayoutUtility.GetLastRect();
            //contentTree.Group = map ? map.Groups.ElementAtOrDefault(map.currentEditorSelectedGroup) : null;
            //contentTree.OnGUI(rect);

            HorizontalLine();

            GUILayout.BeginVertical(GUILayout.Width(300f));
            inspector.OnGUI();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        void HorizontalLine()
        {
            GUIStyle style = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = qGUIUtility.GenerateColorTexture(new Color(0f, 0f, 0f)),
                },
                fixedWidth = 1f,
                stretchHeight = true,
            };

            GUILayout.Box(GUIContent.none, style);
        }
    }
}