#if UNITY_EDITOR
using UnityEditor;
using System;

namespace qASIC.ProjectSettings.Internal
{
    public static class ProjectSettingsImporter
    {
        private static Action[] actions = new Action[]
        {
            () => _ = AudioProjectSettings.Instance,
            () => _ = ConsoleProjectSettings.Instance,
            () => _ = DisplayerProjectSettings.Instance,
            () => _ = InputProjectSettings.Instance,
            () => _ = OptionsProjectSettings.Instance,
        };

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
#if !qASIC_DEV
            for (int i = 0; i < actions.Length; i++)
                actions[i].Invoke();
#endif
        }

        [MenuItem("Window/qASIC/Import Project Settings", priority = 200)]
        public static void Import()
        {
#if qASIC_DEV
            if (!EditorUtility.DisplayDialog("Create default project settings?", "Do you want to rebuild default project settings?", "Ok", "Cancel"))
                return;
#endif

            try
            {
                for (int i = 0; i < actions.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Importing Project Settings", "Hold up", i / (float)actions.Length);
                    actions[i].Invoke();
                }
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Importing failed!", "There was an error while importing project settings.", "Ok");
                throw e;
            }

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Success", "Project Settings imported successfully!", "Ok");
        }
    }
}
#endif