using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

using Settings = qASIC.ProjectSettings.DisplayerProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    class DisplayerSettingsProvider : SettingsProvider
    {
        Settings settings;
        SerializedObject serializedSettings;

        public DisplayerSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.BeginGroup("Debug Displayer");
            DrawProperty(nameof(Settings.debugObjectName), "Object Name");
            DrawProperty(nameof(Settings.debugDisplayerName), "Displayer Name");

            DrawProperty(nameof(Settings.debugTogglerName), "Toggler Name");
            DrawProperty(nameof(Settings.debugTogglerKey), "Toggler Key");

            DrawProperty(nameof(Settings.displayDebugGenerationMessage), "Display Message");
            DrawProperty(nameof(Settings.debugGenerationMessage), "Message");
            DrawProperty(nameof(Settings.debugGenerationMessageColor), "Message Color");

            DrawProperty(nameof(Settings.createDebugInEditor), "Create In Editor");
            DrawProperty(nameof(Settings.createDebugInDeveloperBuild), "Create In Dev Build");
            DrawProperty(nameof(Settings.createDebugInBuild), "Create In Build");
            qGUIInternalUtility.EndGroup();

            serializedSettings.ApplyModifiedProperties();
        }

        void DrawProperty(string property, string label) =>
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(property), new GUIContent(label));

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new DisplayerSettingsProvider("Project/qASIC/Displayer", SettingsScope.Project)
            {
                label = "Info Displayer",
                keywords = new HashSet<string>(new[] { "qASIC", "Displayer", "Info Displayer", "Info" })
            };

            return provider;
        }
    }
}