using UnityEditor;
using UnityEngine;

namespace qASIC.InputManagement.Internal
{
    [CustomEditor(typeof(InputMap))]
    public class InputMapInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (InputMapWindow.DebugMode)
                base.OnInspectorGUI();

            InputMap map = (InputMap)target;
            if (GUILayout.Button("Open editor", GUILayout.Height(24f)))
                InputMapWindow.OpenMap(map);
        }
    }
}