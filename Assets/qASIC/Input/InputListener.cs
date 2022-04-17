using UnityEngine;

namespace qASIC.InputManagement.Menu
{
    [AddComponentMenu("qASIC/Input/Legacy/Input Listener")]
    public class InputListener : MonoBehaviour
    {
        [Message("Input Listener will no longer be updated.", InspectorMessageIconType.notification)]
        public UnityEventKeyCode OnInputRecived = new UnityEventKeyCode();

        bool isListening = false;
        private bool _stopOnKeyPress = true;
        private bool _destroyOnKeyPress = false;

        public void StartListening() => isListening = true;
        public void StopListening() => isListening = false;

        public void StartListening(bool stopOnKeyPress, bool destroy) 
        {
            isListening = true;
            _stopOnKeyPress = stopOnKeyPress;
            _destroyOnKeyPress = destroy;
        }

        private void Update()
        {
            if (!isListening) return;
            foreach (KeyCode key in KeyboardManager.AllKeyCodes)
            {
                if (!Input.GetKeyDown(key)) continue;
                if (_stopOnKeyPress) StopListening();
                if (_destroyOnKeyPress) Destroy(gameObject);
                OnInputRecived.Invoke(key);
                ResetListiner();
                return;
            }
        }

        private void ResetListiner()
        {
            _stopOnKeyPress = true;
            _destroyOnKeyPress = false;
        }
    }
}