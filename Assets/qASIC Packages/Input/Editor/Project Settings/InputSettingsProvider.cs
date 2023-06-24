#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.EditorTools;

using Settings = qASIC.ProjectSettings.InputProjectSettings;
using qASIC.Files;

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
                Input.Map.Internal.InputMapWindow.OpenMapIfNotDirty(settings.map);
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.BeginGroup("Device structure");
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                DrawProperty(nameof(Settings.deviceStructure));
                if (change.changed)
                {
                    serializedSettings.ApplyModifiedProperties();
                    Input.Devices.DeviceManager.Reload();
                }
            }
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.BeginGroup("Saving");
            DrawProperty(nameof(Settings.serializer));
            qGUIInternalUtility.EndGroup();

            qGUIInternalUtility.BeginGroup("Starting arguments");
            qGUIEditorUtility.DrawPropertiesInRange(serializedSettings,
                nameof(Settings.startArgsDisableLoad),
                nameof(Settings.startArgsDisableSave));
            qGUIInternalUtility.EndGroup();

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
                label = "Input",
                keywords = new HashSet<string>(new[] { "qASIC", "Input", "Input Management", "Input System" })
            };

            return provider;
        }
    }
}
#endif