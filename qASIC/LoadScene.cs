using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC
{
    public class LoadScene : MonoBehaviour
    {
        public void Load(string sceneName)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName)) return;
            SceneManager.LoadScene(sceneName);
        }

        public void Load(int index)
        {
            if (!Application.CanStreamedLevelBeLoaded(index)) return;
            SceneManager.LoadScene(index);
        }
    }
}