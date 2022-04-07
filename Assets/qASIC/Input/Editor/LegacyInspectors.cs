#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Tools
{
    [CustomEditor(typeof(InputPreset))]
    public class InputPresetCI : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Input Preset has been replaced with Input Map. For more details click the button bellow.", MessageType.Warning);
            if (Button("More details"))
                Application.OpenURL(@"https://docs.qasictools.com/general/faq/legacy-input-system");
        }
    }

    [CustomEditor(typeof(SetGlobalInputKeys))]
    public class SetGlobalInputKeysInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Set Global Input Keys has been replaced with Input Load. For more details click the button bellow.", MessageType.Warning);
            if (Button("More details"))
                Application.OpenURL(@"https://docs.qasictools.com/general/faq/legacy-input-system");
        }
    }
}
#endif