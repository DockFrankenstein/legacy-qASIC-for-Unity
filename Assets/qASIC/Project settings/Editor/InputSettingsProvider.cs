using UnityEditor;
using UnityEngine;
using qASIC.EditorTools;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.Options;

namespace qASIC.ProjectSettings.Internal
{
    class InputSettingsProvider : SettingsProvider
    {
        SerializedObject serializedSettings;
        InputProjectSettings settings;

        public InputSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = InputProjectSettings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIUtility.DrawqASICBanner();

#if !ENABLE_LEGACY_INPUT_MANAGER
            EditorGUILayout.HelpBox("qASIC input doesn't support the New Input System. Please go to Project Settings/Player and change Active Input Handling to Input Manager or Both.", MessageType.Warning);
            if (GUILayout.Button("Open Project Settings"))
                SettingsService.OpenProjectSettings("Project/Player");
            EditorGUI.BeginDisabledGroup(true);
#else
            EditorGUI.BeginDisabledGroup(false);
#endif
            //Map
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Map", EditorStyles.whiteLargeLabel);

            EditorGUILayout.PropertyField(serializedSettings.FindProperty("map"));

            if (settings.map && GUILayout.Button("Edit map"))
                InputManagement.Map.Internal.InputMapWindow.OpenMapIfNotDirty(settings.map);

            EditorGUILayout.Space();
            GUILayout.EndVertical();

            //Saving
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Saving", EditorStyles.whiteLargeLabel);

            SerializedProperty serializationTypeProperty = serializedSettings.FindProperty("serializationType");

            EditorGUILayout.PropertyField(serializationTypeProperty);

            if ((SerializationType)serializationTypeProperty.intValue != SerializationType.playerPrefs)
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("filePath"));

            EditorGUILayout.Space();
            GUILayout.EndVertical();

            //Starting arguments
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Starting arguments", EditorStyles.whiteLargeLabel);

            settings.startArgsDisableLoad = GUILayout.Toggle(settings.startArgsDisableLoad, new GUIContent("Allow Disabling Loading"));
            settings.startArgsDisableSave = GUILayout.Toggle(settings.startArgsDisableSave, new GUIContent("Allow Disabling Saving"));

            EditorGUILayout.Space();
            GUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();
            serializedSettings.ApplyModifiedProperties();
        }

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