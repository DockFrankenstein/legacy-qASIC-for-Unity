using UnityEngine;
using System;

namespace qASIC.InputManagment
{
    public class InputListiner : MonoBehaviour
    {
        public UnityEventKeyCode onInputRevived = new UnityEventKeyCode();

        private bool isListening = false;
        private bool stopOnKeyPress = true;
        private bool destroyOnKeyPress = true;

        public void StartListening() => isListening = true;
        public void StartListening(bool stop, bool destroy) 
        { isListening = true; stopOnKeyPress = stop; destroyOnKeyPress = destroy; }
        public void StopListening() => isListening = false;

        private void Update()
        {
            if (!isListening) return;
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (stopOnKeyPress) isListening = false;
                    if (destroyOnKeyPress) Destroy(gameObject);
                    onInputRevived.Invoke(key);
                    return;
                }
            }
        }
    }
}