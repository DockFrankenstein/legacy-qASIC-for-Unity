using UnityEngine;
using System.Collections.Generic;
using System;

namespace qASIC.InputManagement.Devices
{
    public class UIMKeyboardDevice : IKeyboardDevice
    {
        public string DeviceName => "Keyboard";
        public Type KeyType => typeof(KeyCode);

        private Dictionary<KeyCode, bool> _keys = new Dictionary<KeyCode, bool>();
        private Dictionary<KeyCode, bool> _keysUp = new Dictionary<KeyCode, bool>();
        private Dictionary<KeyCode, bool> _keysDown = new Dictionary<KeyCode, bool>();
        private Vector2 mousePosition;
        private Vector2 mouseMove;

        public float GetInputValue(int keyIndex)
        {
            KeyCode key = (KeyCode)keyIndex;
            if (!_keys.ContainsKey(key))
                return 0f;

            return _keys[key] ? 1f : 0f;
        }

        public bool GetInputEvent(KeyEventType type, int keyIndex)
        {
            KeyCode key = (KeyCode)keyIndex;
            Dictionary<KeyCode, bool> dictionary = _keys;
            switch (type)
            {
                case KeyEventType.up:
                    dictionary = _keysUp;
                    break;
                case KeyEventType.down:
                    dictionary = _keysDown;
                    break;
            }

            if (!dictionary.ContainsKey(key))
                return false;

            return dictionary[key];
        }

        public Vector2 GetMousePosition() =>
            mousePosition;

        public Vector2 GetMouseMove() =>
            mouseMove;

        public void Initialize()
        {
            foreach (KeyCode key in KeyboardManager.AllKeyCodes)
            {
                _keys.Add(key, false);
                _keysUp.Add(key, false);
                _keysDown.Add(key, false);
            }
        }

        public void Update()
        {
            foreach (var key in KeyboardManager.AllKeyCodes)
            {
                bool keyValue = Input.GetKey(key);
                bool previousValue = _keys[key];
                _keysUp[key] = previousValue && !keyValue;
                _keysDown[key] = !previousValue && keyValue;
                _keys[key] = keyValue;
            }

            Vector2 newMousePosition = Input.mousePosition;
            mouseMove = newMousePosition - mousePosition;
            mousePosition = newMousePosition;
        }
    }
}
