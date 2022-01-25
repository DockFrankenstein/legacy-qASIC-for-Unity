using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.EditorTools;

using Settings = qASIC.ProjectSettings.ConsoleProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    class ConsoleSettingsProvider : SettingsProvider
    {
        Settings settings;
        SerializedObject serializedSettings;

        public ConsoleSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.DrawqASICBanner();

            DrawProperty(nameof(Settings.config));
            EditorGUILayout.Space();

            if (settings.config)
                qGUIUtility.DrawObjectsInspector(settings.config);

            serializedSettings.ApplyModifiedProperties();
        }

        void DrawProperty(string propertyName) =>
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(propertyName));

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new ConsoleSettingsProvider("Project/qASIC/Console", SettingsScope.Project)
            {
                label = "Game Console",
                keywords = new HashSet<string>(new[] { "qASIC", "Console", "Game Console", "Cheat Console", "Cheat", "Dev Console" })
            };

            return provider;
        }
    }
}
