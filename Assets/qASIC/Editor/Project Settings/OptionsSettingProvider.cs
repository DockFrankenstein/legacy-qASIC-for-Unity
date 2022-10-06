#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
            qGUIInternalUtility.DrawPropertyGroup(serializedSettings, "General", new string[]
            {
                nameof(Settings.enableOptionsSystem),
                nameof(Settings.autoInitialize),
            });

            EditorGUI.BeginDisabledGroup(!settings.enableOptionsSystem);

            qGUIInternalUtility.BeginGroup("Saving");
            DrawProperty(nameof(Settings.serializationType));
            if (settings.serializationType == FileManagement.SerializationType.config)
                DrawProperty(nameof(Settings.savePath));
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.DrawPropertyGroup(serializedSettings, "Starting arguments", new string[]
            {
                nameof(Settings.startArgsDisableSave),
                nameof(Settings.startArgsDisableLoad),
                nameof(Settings.startArgsDisableInit),
            });

            EditorGUI.EndDisabledGroup();
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
#endif