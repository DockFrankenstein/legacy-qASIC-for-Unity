using UnityEngine;
using System.Collections.Generic;
using System;

namespace qASIC.InputManagement.Devices
{
    public class UIMKeyboardDevice : IKeyboardDevice
    {
        public string DeviceName => "Keyboard";
        public Type KeyType => typeof(KeyCode);

        private Dictionary<string, bool> _keys = new Dictionary<string, bool>();
        private Dictionary<string, bool> _keysUp = new Dictionary<string, bool>();
        private Dictionary<string, bool> _keysDown = new Dictionary<string, bool>();
        private Vector2 mousePosition;
        private Vector2 mouseMove;

        public float GetInputValue(string keyPath)
        {
            if (!_keys.ContainsKey(keyPath))
                return 0f;

            return _keys[keyPath] ? 1f : 0f;
        }

        public InputEventType GetInputEvent(string keyPath)
        {
            InputEventType type = InputEventType.None;

            if (!_keys.ContainsKey(keyPath))
                return type;

            if (_keys[keyPath])
                type = InputEventType.Pressed;

            if (_keysUp[keyPath])
                type = InputEventType.Up;

            if (_keysDown[keyPath])
                type = InputEventType.Down;

            return type;
        }

        public Vector2 GetMousePosition() =>
            mousePosition;

        public Vector2 GetMouseMove() =>
            mouseMove;

        public void Initialize()
        {
            foreach (KeyCode key in KeyboardManager.AllKeyCodes)
            {
                string keyName = GetKeyName(key);
                _keys.Add(keyName, false);
                _keysUp.Add(keyName, false);
                _keysDown.Add(keyName, false);
            }
        }

        public void Update()
        {
            foreach (var key in KeyboardManager.AllKeyCodes)
            {
                string keyName = GetKeyName(key);
                bool keyValue = Input.GetKey(key);
                bool previousValue = _keys[keyName];
                _keysUp[keyName] = previousValue && !keyValue;
                _keysDown[keyName] = !previousValue && keyValue;
                _keys[keyName] = keyValue;
            }

            Vector2 newMousePosition = Input.mousePosition;
            mouseMove = newMousePosition - mousePosition;
            mousePosition = newMousePosition;
        }

        string GetKeyName(KeyCode key) =>
            $"key_keyboard/{key}";
    }
}
