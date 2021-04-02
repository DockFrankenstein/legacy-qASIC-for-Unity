#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace qASIC.Console.Tools
{
    [CustomEditor(typeof(GameConsoleInterface))]
    public class GameConsoleInterfaceCI : Editor
    {
        public override void OnInspectorGUI()
        {
            GameConsoleInterface script = (GameConsoleInterface)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Reload config")) script.AssignConfig();
            if (GUILayout.Button("Refresh")) script.RefreshLogs();
        }
    }
}
#endif