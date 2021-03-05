using UnityEngine;
using System;

namespace qASIC.InputManagment.Menu
{
    public class InputListiner : MonoBehaviour
    {
        public UnityEventKeyCode onInputRecived = new UnityEventKeyCode();

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
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (!Input.GetKeyDown(key)) continue;
                if (_stopOnKeyPress) StopListening();
                if (_destroyOnKeyPress) Destroy(gameObject);
                onInputRecived.Invoke(key);
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