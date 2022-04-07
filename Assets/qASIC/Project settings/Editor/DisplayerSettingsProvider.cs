#if UNITY_EDITOR
using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;
using qASIC.EditorTools;

using Settings = qASIC.ProjectSettings.DisplayerProjectSettings;

namespace qASIC.ProjectSettings.Internal
{
    class DisplayerSettingsProvider : SettingsProvider
    {
        Settings settings;
        SerializedObject serializedSettings;

        public DisplayerSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = Settings.Instance;
            serializedSettings = new SerializedObject(settings);
        }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.BeginGroup("Debug Displayer");
            qGUIEditorUtility.DrawObjectsProperties(serializedSettings);
            qGUIInternalUtility.EndGroup();

            serializedSettings.ApplyModifiedProperties();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new DisplayerSettingsProvider("Project/qASIC/Displayer", SettingsScope.Project)
            {
                label = "Info Displayer",
                keywords = new HashSet<string>(new[] { "qASIC", "Displayer", "Info Displayer", "Info" })
            };

            return provider;
        }
    }
}
#endif