using UnityEditor;
using qASIC.EditorTools;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.Options;

namespace qASIC.ProjectSettings.Internal
{
    class InputSettingsProvider : SettingsProvider
    {
        SerializedObject settings;

        public InputSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = new SerializedObject(InputProjectSettings.Instance);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIUtility.DrawqASICBanner();

#if !ENABLE_LEGACY_INPUT_MANAGER
            EditorGUILayout.HelpBox("qASIC input doesn't support the New Input System. Please go to Project Settings/Player and change Active Input Handling to Input Manager or Both.", MessageType.Warning);
            if (GUILayout.Button("Open Project Settings"))
                SettingsService.OpenProjectSettings("Project/Player");
#else
            EditorGUILayout.PropertyField(settings.FindProperty("map"));

            SerializedProperty serializationTypeProperty = settings.FindProperty("serializationType");

            EditorGUILayout.PropertyField(serializationTypeProperty);

            if ((SerializationType)serializationTypeProperty.intValue != SerializationType.playerPrefs)
                EditorGUILayout.PropertyField(settings.FindProperty("filePath"));

            settings.ApplyModifiedProperties();
#endif
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