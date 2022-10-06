#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using qASIC.Internal;

namespace qASIC.ProjectSettings.Internal
{
	class qASICSettingsProvider : SettingsProvider
	{
		public qASICSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.DrawqASICBanner();

            qGUIInternalUtility.BeginGroup("Version");

            EditorGUILayout.LabelField($"qASIC: {Info.Version}");
            EditorGUILayout.LabelField($"Game Console: {Info.ConsoleVersion}");
            EditorGUILayout.LabelField($"Info Displayer: {Info.DisplayerVersion}");
            EditorGUILayout.LabelField($"Options System: {Info.OptionsVersion}");
            EditorGUILayout.LabelField($"Input Management: {Info.InputVersion}");
            EditorGUILayout.LabelField($"Audio Management: {Info.AudioVersion}");
            EditorGUILayout.LabelField($"File Management: {Info.FileVersion}");

            qGUIInternalUtility.EndGroup();

            EditorGUILayout.Space();
            if (GUILayout.Button("Re-import project settings", GUILayout.Height(24f)))
                ProjectSettingsImporter.Import();
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new qASICSettingsProvider("Project/qASIC", SettingsScope.Project)
            {
                label = "qASIC",
                keywords = new HashSet<string>(new[] { "qASIC" })
            };

            return provider;
        }
    }
}
#endif