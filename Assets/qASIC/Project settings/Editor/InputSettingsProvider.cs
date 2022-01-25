using UnityEditor;
using UnityEngine;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.Options;

using Settings = qASIC.ProjectSettings.InputProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    class InputSettingsProvider : SettingsProvider
    {
        SerializedObject serializedSettings;
        Settings settings;

        public InputSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.DrawqASICBanner();

#if !ENABLE_LEGACY_INPUT_MANAGER
            EditorGUILayout.HelpBox("qASIC input doesn't support the New Input System. Please go to Project Settings/Player and change Active Input Handling to Input Manager or Both.", MessageType.Warning);
            if (GUILayout.Button("Open Project Settings"))
                SettingsService.OpenProjectSettings("Project/Player");
            EditorGUI.BeginDisabledGroup(true);
#else
            EditorGUI.BeginDisabledGroup(false);
#endif
            //Map
            qGUIInternalUtility.BeginGroup("Map");
            DrawProperty(nameof(Settings.map));
            if (settings.map && GUILayout.Button("Edit map", qGUIInternalUtility.Styles.OpenButton))
                InputManagement.Map.Internal.InputMapWindow.OpenMapIfNotDirty(settings.map);
            qGUIInternalUtility.EndGroup();

            //Disable rest if there is no map assigned
            EditorGUI.BeginDisabledGroup(!settings.map);

            //Saving
            qGUIInternalUtility.BeginGroup("Saving");
            SerializedProperty serializationTypeProperty = serializedSettings.FindProperty(nameof(Settings.serializationType));
            EditorGUILayout.PropertyField(serializationTypeProperty);
            if ((SerializationType)serializationTypeProperty.intValue != SerializationType.playerPrefs)
                DrawProperty(nameof(Settings.filePath));
            qGUIInternalUtility.EndGroup();

            //Starting arguments
            qGUIInternalUtility.BeginGroup("Starting arguments");
            settings.startArgsDisableLoad = GUILayout.Toggle(settings.startArgsDisableLoad, new GUIContent("Allow Disabling Loading"));
            settings.startArgsDisableSave = GUILayout.Toggle(settings.startArgsDisableSave, new GUIContent("Allow Disabling Saving"));
            qGUIInternalUtility.EndGroup();

            //End no map assigned disabled group
            EditorGUI.EndDisabledGroup();

            //End not supported input handler disabled group
            EditorGUI.EndDisabledGroup();

            serializedSettings.ApplyModifiedProperties();
        }

        void DrawProperty(string property) =>
            EditorGUILayout.PropertyField(serializedSettings.FindProperty(property));

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new InputSettingsProvider("Project/qASIC/Input", SettingsScope.Project)
            {
                label = "Input Management",
                keywords = new HashSet<string>(new[] { "qASIC", "Input", "Input Management", "Input System" })
            };

            return provider;
        }
    }
}