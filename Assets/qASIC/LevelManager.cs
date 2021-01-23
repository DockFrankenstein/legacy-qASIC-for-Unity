using UnityEngine.SceneManagement;
using UnityEngine;
using qASIC.Console;

namespace qASIC
{
    public class LevelManager : MonoBehaviour
    {
        private static Color levelColor = new Color(0f, 0.9f, 1f);
        private static Color errorColor = new Color(1f, 0f, 0f);

        public static void LoadScene(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                GameConsoleController.Log("Loaded scene <b>" + sceneName + "</b>", "");
            }
            else
                GameConsoleController.Log("Scene <b>" + sceneName + "</b> does not exist!", "Error");
        }

        public static void LoadScene(int index)
        {
            if (SceneManager.sceneCountInBuildSettings > index)
            {
                LoadScene(SceneManager.GetSceneByBuildIndex(index).name);
            }
            else
                GameConsoleController.Log("Scene <b>" + index + "</b> is out of range!", "Error");
        }
    }
}