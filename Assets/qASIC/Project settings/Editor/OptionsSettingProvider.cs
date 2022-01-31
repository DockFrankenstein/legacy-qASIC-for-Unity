using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.EditorTools;

using Settings = qASIC.ProjectSettings.OptionsProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    public class OptionsSettingProvider : SettingsProvider
    {
        Settings settings;
        SerializedObject serializedSettings;

        public OptionsSettingProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.BeginGroup("Saving");
            DrawProperty(nameof(Settings.serializationType));
            if (settings.serializationType != FileManagement.SerializationType.playerPrefs)
                DrawProperty(nameof(Settings.savePath));

            qGUIInternalUtility.EndGroup();

            serializedSettings.ApplyModifiedProperties();
        }

        void DrawProperty(string propertyName, bool disable = false)
        {
            EditorGUI.BeginDisabledGroup(disable);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(propertyName));
            EditorGUI.EndDisabledGroup();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new OptionsSettingProvider("Project/qASIC/Options", SettingsScope.Project)
            {
                label = "Options",
                keywords = new HashSet<string>(new[] { "qASIC", "Options", "Options System" })
            };

            return provider;
        }
    }
}