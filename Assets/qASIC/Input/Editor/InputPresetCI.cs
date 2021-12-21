using UnityEngine;
using UnityEditor;
using static qASIC.UnityEditor.qGUIUtility;
using static UnityEngine.GUILayout;

namespace qASIC.InputManagement.Tools
{
    [CustomEditor(typeof(InputPreset))]
    public class InputPresetCI : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawMessageBox("Input Preset has been replaced with Input Map. For more details click the button bellow", InspectorMessageIconType.warning);
            if (Button("More details"))
                Application.OpenURL(@"https://docs.qasictools.com/");
        }
    }
}