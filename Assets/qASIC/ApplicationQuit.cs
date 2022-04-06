using UnityEngine;
using UnityEngine.UI;

namespace qASIC
{
    public class ApplicationQuit : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Reset()
        {
            Button button = GetComponent<Button>();
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