using UnityEngine;
using UnityEngine.UI;
using qASIC.Console;

namespace qASIC
{
    public class ExecuteCommand : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Reset()
        {
            Button button = GetComponent<Button>();
            if (button == null) return;

            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(button.onClick, Execute, "");
        }
#endif

        public void Execute(string cmd)
        {
            GameConsoleController.RunCommand(cmd);
        }
    }
}
