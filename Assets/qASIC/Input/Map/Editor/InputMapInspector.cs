#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.EditorTools.Internal;

namespace qASIC.InputManagement.Map.Internal
{
    [CustomEditor(typeof(InputMap))]
    public class InputMapInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (InputMapWindow.DebugMode)
                base.OnInspectorGUI();

            InputMap map = (InputMap)target;
            if (GUILayout.Button("Open editor", qGUIInternalUtility.Styles.OpenButton))
                InputMapWindow.OpenMapIfNotDirty(map);
        }
    }
}
#endif