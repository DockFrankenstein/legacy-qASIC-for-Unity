using UnityEditor;
using UnityEngine;

namespace qASIC.InputManagement.Internal
{
    public class InputMapWindow : EditorWindow
    {
        [SerializeField] Texture2D icon;

        static InputMap map;

        InputMapEditorGroupDisplayer groupUI = new InputMapEditorGroupDisplayer();
        InputMapEditorActionDisplayer actionUI = new InputMapEditorActionDisplayer();
        InputMapEditorKeysDisplayer keyUI = new InputMapEditorKeysDisplayer();

        const string mapPrefsKey = "qASIC_editor_input_map";

        [MenuItem("Window/qASIC/Input Map Editor")]
        static void OpenWindow()
        {
            InputMapWindow window = GetEdtorWindow();
            window.titleContent.image = window.icon;
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

        public static InputMapWindow GetEdtorWindow() =>
            (InputMapWindow)GetWindow(typeof(InputMapWindow), false, "Input Map Editor");

        private void OnEnable()
        {
            LoadMap();
            actionUI = new InputMapEditorActionDisplayer();
            groupUI = new InputMapEditorGroupDisplayer();
            keyUI = new InputMapEditorKeysDisplayer();
            groupUI.map = map;
        }

        private void OnGUI()
        {
            bool isMapSelected = map != null;
            int group = isMapSelected ? map.currentEditorSelectedGroup : -1;
            bool isGroupSelected = group >= 0 && group < map.Groups.Count;
            int action = isGroupSelected ? map.Groups[group].currentEditorSelectedAction : -1;
            bool isActionSelected = action >= 0 && action < map.Groups[group].actions.Count;

            actionUI.group = isGroupSelected ? map.Groups[group] : null;
            keyUI.action = isActionSelected ? map.Groups[group].actions[action] : null;

            if (isMapSelected)
                map.defaultGroup = EditorGUILayout.Popup(map.defaultGroup, map.GetGroupNames());

            groupUI.OnGUI();
            actionUI.OnGUI();
            keyUI.OnGUI();

            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(map);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void HorizontalLine()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();

            GUIStyle style = new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = texture,
                },
                fixedWidth = 1f,
                stretchHeight = true,
                margin = new RectOffset(3, 3, 0, 0),
            };

            GUILayout.Box(GUIContent.none, style);
        }

        Texture2D GenerateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}