#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using qASIC.EditorTools;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using qASIC.FileManagement;

using Settings = qASIC.ProjectSettings.AudioProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    class AudioSettingsProvider : SettingsProvider
    {
        Settings settings;
        SerializedObject serializedSettings;

        public AudioSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.BeginGroup("General");
            DrawProperty(nameof(Settings.enableAudioManager));
            qGUIInternalUtility.EndGroup();

            using (new EditorGUI.DisabledScope(!settings.enableAudioManager))
            {
                qGUIInternalUtility.BeginGroup("Creating");
                DrawInRange(nameof(Settings.managerName), nameof(Settings.createOnUse));
                qGUIInternalUtility.EndGroup();


                qGUIInternalUtility.BeginGroup("Logging creation");
                DrawProperty(nameof(Settings.logCreation));
                DrawInRange(nameof(Settings.creationLogMessage), nameof(Settings.creationLogColor), !settings.logCreation);

                DrawProperty(nameof(Settings.logCreationError));
                DrawInRange(nameof(Settings.creationErrorMessage), nameof(Settings.creationErrorColor), !settings.logCreationError);
                qGUIInternalUtility.EndGroup();


                qGUIInternalUtility.BeginGroup("Saving");
                if (settings.serializationType == SerializationType.playerPrefs)
                    EditorGUILayout.HelpBox("The Audio Manager does not support player prefs!", MessageType.Error);

                DrawProperty(nameof(Settings.serializationType));
                if (settings.serializationType == SerializationType.config)
                    DrawProperty(nameof(Settings.savePath));
                qGUIInternalUtility.EndGroup();


                qGUIInternalUtility.BeginGroup("Other");
                DrawInRange(nameof(Settings.roundValues), nameof(Settings.roundValues));
                qGUIInternalUtility.EndGroup();
            }

            serializedSettings.ApplyModifiedProperties();
        }

        void DrawProperty(string propertyName, bool disable = false)
        {
            EditorGUI.BeginDisabledGroup(disable);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(propertyName));
            EditorGUI.EndDisabledGroup();
        }

        void DrawInRange(string startProperty, string endProperty, bool disable = false)
        {
            EditorGUI.BeginDisabledGroup(disable);
            qGUIEditorUtility.DrawPropertiesInRange(serializedSettings, startProperty, endProperty);
            EditorGUI.EndDisabledGroup();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new AudioSettingsProvider("Project/qASIC/Audio", SettingsScope.Project)
            {
                label = "Audio",
                keywords = new HashSet<string>(new[] { "qASIC", "Audio", "Audio Management", "Audio Manager" })
            };

            return provider;
        }
    }
}

#endif