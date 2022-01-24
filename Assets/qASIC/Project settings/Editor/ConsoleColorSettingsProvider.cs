using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.Console;
using UnityEngine;
using qASIC.EditorTools;

namespace qASIC.ProjectSettings.Internal
{
    class ConsoleColorSettingsProvider : SettingsProvider
    {
        ConsoleProjectSettings settings;

        public ConsoleColorSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ConsoleProjectSettings.Instance;
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.BeginGroup("Theme");

            ColorThemeField();

            qGUIInternalUtility.EndGroup();

            if (!settings.config?.colorTheme) return;

            EditorGUILayout.Space();
            qGUIUtility.DrawObjectsInspector(settings.config.colorTheme);
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