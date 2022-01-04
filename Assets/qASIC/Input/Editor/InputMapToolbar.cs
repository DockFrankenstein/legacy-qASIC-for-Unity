using UnityEngine;
using UnityEditor;

using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Internal
{
    public class InputMapToolbar
    {
        public InputMap map;

        GenericMenu debugMenu = new GenericMenu();

        public InputMapToolbar()
        {
            debugMenu.AddItem(new GUIContent("Close map"), false, InputMapWindow.CloseMap);
        }

        public void OnGUI()
        {
            BeginHorizontal(EditorStyles.toolbar);

            DrawMenus();
            EditorGUILayout.Space();
            Label(map ? map.name : "Not loaded");

            FlexibleSpace();

            if (Button("Show in folder", EditorStyles.toolbarButton))
                ShowInFolder();

            if (Toggle(map && map.autoSave, "Auto save", EditorStyles.toolbarButton) != (map && map.autoSave) && map)
                map.autoSave = !map.autoSave;

            if (Button("Save", EditorStyles.toolbarButton))
                Save();

            EditorGUILayout.Space();
            EndHorizontal();
        }

        void DrawMenus()
        {
            if (Button("Debug", EditorStyles.toolbarButton))
                debugMenu.ShowAsContext();
        }

        void ShowInFolder()
        {
            if (map == null) return;
            Selection.SetActiveObjectWithContext(map, null);
            EditorGUIUtility.PingObject(map);
            EditorUtility.FocusProjectWindow();
        }

        void Save()
        {
            if (map == null) return;
            EditorUtility.SetDirty(map);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}