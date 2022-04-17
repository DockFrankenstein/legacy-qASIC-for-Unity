using UnityEngine.SceneManagement;
using UnityEngine;

namespace qASIC
{
    [AddComponentMenu("qASIC/Menu/Load Scene")]
    public class LoadScene : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Reset()
        {
            UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
            if (button == null) return;

            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(button.onClick, Load, "");
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

        public void LoadPrevious() =>
            Load(SceneManager.GetActiveScene().buildIndex - 1);

        public void LoadNext() =>
            Load(SceneManager.GetActiveScene().buildIndex + 1);
    }
}