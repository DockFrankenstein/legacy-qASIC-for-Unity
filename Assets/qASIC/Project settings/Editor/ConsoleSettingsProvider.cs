using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.EditorTools;

namespace qASIC.ProjectSettings.Internal
{
    class ConsoleSettingsProvider : SettingsProvider
    {
        ConsoleProjectSettings settings;
        SerializedObject serializedSettings;

        public ConsoleSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ConsoleProjectSettings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.DrawqASICBanner();

            qGUIInternalUtility.BeginGroup("Config");
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("config"));
            qGUIInternalUtility.EndGroup();

            EditorGUILayout.Space();

            if (settings.config)
                qGUIUtility.DrawObjectsInspector(settings.config);

            serializedSettings.ApplyModifiedProperties();
        }

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
