#if UNITY_EDITOR
using qASIC.FileManaging;
using UnityEditor;
using UnityEngine;

namespace qASIC.Console.Tools
{
    public class GameConsoleConfigWindow : EditorWindow
    {
        [MenuItem("Window/qASIC/Console configuration")]
        public static void ShowWindow()
        {
            GameConsoleConfigWindow window = (GameConsoleConfigWindow)GetWindow(typeof(GameConsoleConfigWindow));
            window.Show();
        }

        public void OnEnable()
        {
            GameConsoleController.LoadConfig();
        }

        public void Save()
        {
            string path = GameConsoleController.GetConfigPath();
            ConfigController.SaveSetting(path, "Use unity", GameConsoleController.useUnityConsole.ToString().ToLower());
        }

        public void OnGUI()
        {
            GameConsoleController.useUnityConsole = GUILayout.Toggle(GameConsoleController.useUnityConsole, "Use unity console");
            if (GUILayout.Button("Save")) Save();
        }
    }
}
#endif