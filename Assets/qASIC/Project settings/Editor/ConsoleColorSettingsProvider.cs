#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.Console.Internal;
using UnityEngine;
using qASIC.EditorTools;

using Settings = qASIC.ProjectSettings.ConsoleProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    class ConsoleColorSettingsProvider : SettingsProvider
    {
        Settings settings;

        public ConsoleColorSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
        }

        public override void OnGUI(string searchContext)
        {
            bool isConfigReadOnly = settings.config && settings.config.IsReadOnly;

            if (isConfigReadOnly)
            {
                EditorGUILayout.HelpBox("This is a read only configuration. If you want to modify values, please create a new one.", MessageType.Warning);
                EditorGUILayout.Space();
            }

            EditorGUI.BeginDisabledGroup(isConfigReadOnly);
            ColorThemeField();
            EditorGUI.EndDisabledGroup();

            if (!isConfigReadOnly && settings.config?.colorTheme)
                qGUIEditorUtility.DrawObjectsInspector(settings.config.colorTheme);
        }

        void ColorThemeField()
        {
            if (!settings.config)
                EditorGUILayout.HelpBox("You have to asign a console config first in qASIC/Game Console", MessageType.Info);

            EditorGUI.BeginDisabledGroup(!settings.config);
            Object obj = EditorGUILayout.ObjectField("Color theme", settings.config?.colorTheme, typeof(GameConsoleTheme), false);
            if (settings.config)
                settings.config.colorTheme = obj as GameConsoleTheme;
            EditorGUI.EndDisabledGroup();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new ConsoleColorSettingsProvider("Project/qASIC/Console/Colors", SettingsScope.Project)
            {
                label = "Colors",
                keywords = new HashSet<string>(new[] { "qASIC" })
            };

            return provider;
        }
    }
}
#endif