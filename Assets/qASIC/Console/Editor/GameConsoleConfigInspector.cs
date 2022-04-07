#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools.Internal;
using qASIC.EditorTools;

using Config = qASIC.Console.Internal.GameConsoleConfig;

namespace qASIC.Console.Internal
{
    [CustomEditor(typeof(GameConsoleConfig))]
    public class GameConsoleConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            bool isReadOnly = (target as Config).IsReadOnly;

            if (isReadOnly)
                EditorGUILayout.HelpBox("This is a read only configuration. If you want to modify values, please create a new one.", MessageType.Warning);

            EditorGUI.BeginDisabledGroup(isReadOnly);

            //Basic
            DrawGroup("Color theme", new string[] { "colorTheme" });

            //Preferences
            DrawGroup("Preferences", new string[] { "textTreeStyle" });

            //Built in commands
            qGUIInternalUtility.BeginGroup("Built in commands");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject, nameof(Config.clearCommand), nameof(Config.fovCommand));
            qGUIInternalUtility.EndGroup();

            //Unity
            qGUIInternalUtility.BeginGroup("Unity");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject, nameof(Config.logToUnity), nameof(Config.logUnityMessagesToConsole));
            qGUIInternalUtility.EndGroup();

            //Version command
            qGUIInternalUtility.BeginGroup("Version command");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject, nameof(Config.displayGameVersion), nameof(Config.displayQasicVersion));
            qGUIInternalUtility.EndGroup();

            //Specs command
            qGUIInternalUtility.BeginGroup("Specs command");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject, nameof(Config.displayCpu), nameof(Config.displaySystem));
            qGUIInternalUtility.EndGroup();

            //Help command
            qGUIInternalUtility.BeginGroup("Help command");
            qGUIEditorUtility.DrawPropertiesInRange(serializedObject, nameof(Config.showDetailedHelp), nameof(Config.pageCommandLimit));
            qGUIInternalUtility.EndGroup();

            serializedObject.ApplyModifiedProperties();

            EditorGUI.EndDisabledGroup();
        }

        void DrawGroup(string label, string[] properties) =>
            qGUIInternalUtility.DrawPropertyGroup(serializedObject, label, properties);
    }
}
#endif