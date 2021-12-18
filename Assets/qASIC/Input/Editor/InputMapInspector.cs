using UnityEditor;
using UnityEngine;

namespace qASIC.InputManagement.Internal
{
    [CustomEditor(typeof(InputMap))]
    public class InputMapInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InputMap map = (InputMap)target;
            if (GUILayout.Button("Open editor"))
                InputMapWindow.OpenMap(map);
        }
    }
}