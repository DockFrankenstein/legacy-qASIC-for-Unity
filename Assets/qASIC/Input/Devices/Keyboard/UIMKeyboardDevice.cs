using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

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

        public string GetAnyKeyDown()
        {
            var downButtons = _keysDown
                .Where(x => x.Value);

            return downButtons.FirstOrDefault().Key;
        }

        public Vector2 GetMousePosition() =>
            mousePosition;

        public Vector2 GetMouseMove() =>
            mouseMove;

        static readonly KeyCode[] KeysToIgnore = new KeyCode[]
        {
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.Mouse2,
            KeyCode.Mouse3,
            KeyCode.Mouse4,
            KeyCode.Mouse5,
            KeyCode.Mouse6,
        };

        private static KeyCode[] _avaliableKeys = null;
        private static KeyCode[] _AvaliableKeys
        {
            get
            {
                if (_avaliableKeys == null)
                    _avaliableKeys = KeyboardManager.AllKeyCodes
                        .Where(x => !KeysToIgnore.Contains(x))
                        .ToArray();

                return _avaliableKeys;
            }
        }


        public void Initialize()
        {
            foreach (KeyCode key in _AvaliableKeys)
            {
                string keyName = GetKeyName(key);
                _keys.Add(keyName, false);
                _keysUp.Add(keyName, false);
                _keysDown.Add(keyName, false);
            }
        }

        public void Update()
        {
            foreach (var key in _AvaliableKeys)
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
