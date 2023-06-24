using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace qASIC.Input.Devices
{
    public class UIMKeyboardDevice : InputDevice, IKeyboardDevice
    {
        public override string DeviceName { get => "Keyboard"; set { } }

        public override Type KeyType => typeof(KeyCode);
        public override bool RuntimeOnly => false;

        public override Dictionary<string, float> Values => _values;

        private Dictionary<string, bool> _keys = new Dictionary<string, bool>();
        private Dictionary<string, bool> _keysUp = new Dictionary<string, bool>();
        private Dictionary<string, bool> _keysDown = new Dictionary<string, bool>();
        private Dictionary<string, float> _values = new Dictionary<string, float>();
        private Vector2 mousePosition;
        private Vector2 mouseMove;

        public override float GetInputValue(string keyPath)
        {
            if (!_keys.ContainsKey(keyPath))
                return 0f;

            return _keys[keyPath] ? 1f : 0f;
        }

        public override InputEventType GetInputEvent(string keyPath)
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

        public override string GetAnyKeyDown()
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
                    _avaliableKeys = UIMKeyboardProvider.AllKeyCodes
                        .Where(x => !KeysToIgnore.Contains(x))
                        .ToArray();

                return _avaliableKeys;
            }
        }


        public override void Initialize()
        {
            _keys.Clear();
            _keysUp.Clear();
            _keysDown.Clear();
            _values.Clear();

            foreach (KeyCode key in _AvaliableKeys)
            {
                string keyName = GetKeyName(key);
                _keys.Add(keyName, false);
                _keysUp.Add(keyName, false);
                _keysDown.Add(keyName, false);
                _values.Add(keyName, 0f);
            }
        }

        public override void Update()
        {
            foreach (var key in _AvaliableKeys)
            {
                string keyName = GetKeyName(key);
                bool keyValue = UnityEngine.Input.GetKey(key);
                bool previousValue = _keys[keyName];
                _keysUp[keyName] = previousValue && !keyValue;
                _keysDown[keyName] = !previousValue && keyValue;
                _keys[keyName] = keyValue;
                _values[keyName] = _keys[keyName] ? 1f : 0f;
            }

            Vector2 newMousePosition = UnityEngine.Input.mousePosition;
            mouseMove = newMousePosition - mousePosition;
            mousePosition = newMousePosition;
        }

        string GetKeyName(KeyCode key) =>
            $"key_keyboard/{key}";
    }
}
