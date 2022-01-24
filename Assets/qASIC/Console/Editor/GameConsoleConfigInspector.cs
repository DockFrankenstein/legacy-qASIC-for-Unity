using UnityEditor;
using qASIC.EditorTools.Internal;

namespace qASIC.Console.Tools.Internal
{
    [CustomEditor(typeof(GameConsoleConfig))]
    public class GameConsoleConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            bool isReadOnly = (target as GameConsoleConfig).IsReadOnly;

            if (isReadOnly)
                EditorGUILayout.HelpBox("This is a read only configuration. If you want to modify values, please create a new one.", MessageType.Warning);

            EditorGUI.BeginDisabledGroup(isReadOnly);

            //Basic
            DrawGroup("Color theme", new string[] { "colorTheme" });

            //Built in commands
            DrawGroup("Built in commands", new string[]
            {
                "clearCommand",
                "echoCommand",
                "exitCommand",
                "helpCommand",
                "inputCommand",
                "optionCommand",
                "audioCommand",
                "sceneCommand",
                "sceneListCommand",
                "versionCommand",
                "specsCommand",
                "debugDisplayerCommand",
                "clearDebugDisplayerCommand",
                "timeScaleCommand",
                "fovCommand",
            });

            //Unity
            DrawGroup("Unity", new string[]
            {
                "logToUnity",
                "logUnityErrorsToConsole",
                "logUnityAssertsToConsole",
                "logUnityExceptionsToConsole",
                "logUnityWarningsToConsole",
                "logUnityMessagesToConsole",
            });

            //Version command
            DrawGroup("Version command", new string[]
            {
                "displayGameVersion",
                "displayUnityVerion",
                "displayQasicVersion",
            });

            //Specs command
            DrawGroup("Specs command", new string[]
            {
                "displayCpu",
                "displayCpuThreads",
                "displayGpu",
                "displayMemory",
                "displaySystem",
            });

            //Help command
            DrawGroup("Help command", new string[]
            {
                "showDetailedHelp",
                "usePageCommandLimit",
                "pageCommandLimit",
            });

            serializedObject.ApplyModifiedProperties();

            EditorGUI.EndDisabledGroup();
        }

        void DrawGroup(string label, string[] properties) =>
            qGUIInternalUtility.DrawPropertyGroup(serializedObject, label, properties);
    }
}