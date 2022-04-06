using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC
{
    public class LoadScene : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Reset()
        {
            UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
            if (button == null) return;

            UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, () => Load(""));
        }
#endif

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