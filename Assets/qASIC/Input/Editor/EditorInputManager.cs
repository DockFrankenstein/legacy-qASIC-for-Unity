using UnityEditor;
using qASIC.InputManagement.Map;

namespace qASIC.InputManagement.Internal
{
    public static class EditorInputManager
    {
        const string mapPrefsKey = "qASIC_input_map_editor_map";

        [InitializeOnLoadMethod]
        static void LoadMap()
        {
            if (!EditorPrefs.HasKey(mapPrefsKey)) return;

            string mapPath = EditorPrefs.GetString(mapPrefsKey);
            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(mapPath))) return;
            Map = (InputMap)AssetDatabase.LoadAssetAtPath(mapPath, typeof(InputMap));
        }

        public static InputMap Map { get; private set; }

        public static void SetMap(InputMap map)
        {
            Map = map;
            EditorPrefs.SetString(mapPrefsKey, AssetDatabase.GetAssetPath(Map));
        }

        public static void RemoveMap()
        {
            Map = null;
            EditorPrefs.DeleteKey(mapPrefsKey);
        }
    }
}