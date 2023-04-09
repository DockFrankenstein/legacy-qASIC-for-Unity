using qASIC.Input.Map;
using qASIC.ProjectSettings;
using UnityEngine;

namespace qASIC.Input.Internal
{
    public static class EditorInputManager
    {
        [UnityEditor.InitializeOnLoadMethod]
        private static void Initialize()
        {
            InputLoad.OnEditorMapChange += map =>
            {
                Map = map == null ? InputProjectSettings.Instance.map : map;
            };

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private static void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            Map = InputProjectSettings.Instance.map;
        }

        public static InputMap Map { get; set; } = InputProjectSettings.Instance.map;
    }
}