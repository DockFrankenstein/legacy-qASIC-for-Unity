#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools.Internal;
using qASIC.EditorTools;

namespace qASIC.Console.Internal
{
    [CustomEditor(typeof(GameConsoleInterface))]
    public class GameConsoleInterfaceCI : Editor
    {
        public override void OnInspectorGUI()
        {
            GameConsoleInterface script = (GameConsoleInterface)target;

            qGUIInternalUtility.DrawqASICBanner();

            DrawContents();
            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh"))
                script.RefreshLogs();
        }

        void DrawContents()
        {
            SerializedProperty consoleConfigProperty = serializedObject.FindProperty("ConsoleConfig");

            qGUIEditorUtility.DrawPropertiesFromStart(serializedObject, "toggler");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Manual assign", EditorStyles.boldLabel);

            if (consoleConfigProperty.objectReferenceValue != null)
                EditorGUILayout.HelpBox("Instead of manually asigning configuration from the interface, you can set it up in Project settings/qASIC/Game Console", MessageType.Info);

            EditorGUILayout.PropertyField(consoleConfigProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif