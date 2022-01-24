using UnityEditor;
using qASIC.EditorTools.Internal;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace qASIC.ProjectSettings.Internal
{
	class qASICSettingsProvider : SettingsProvider
	{
		public qASICSettingsProvider(string path, SettingsScope scopes) : base(path, scopes) { }

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			//settings = new SerializedObject(InputProjectSettings.Instance);
		}

        public override void OnGUI(string searchContext)
        {
            qGUIInternalUtility.DrawqASICBanner();
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