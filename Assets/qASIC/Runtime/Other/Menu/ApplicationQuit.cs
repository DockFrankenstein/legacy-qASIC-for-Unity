using UnityEngine;

namespace qASIC
{
    [AddComponentMenu("qASIC/Menu/Application Quit")]
    public class ApplicationQuit : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Reset()
        {
            UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
            if (button == null) return;

            UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, Quit);
        }
#endif

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}