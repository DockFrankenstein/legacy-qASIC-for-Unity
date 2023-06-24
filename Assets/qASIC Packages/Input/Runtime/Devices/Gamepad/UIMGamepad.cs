using System;
using System.Collections.Generic;
using qASIC.Input.UIM;
using qASIC.ProjectSettings;
using UnityEngine;
using System.Linq;
using qASIC.Internal;

namespace qASIC.Input.Devices
{
    public class UIMGamepad : GamepadDevice, ILeftTriggerDeadZone
    {
        public UIMGamepad() { }

        public UIMGamepad(string deviceName)
        {
            _deviceName = deviceName;
        }

        public UIMGamepad(string deviceName, int managerJoystickIndex)
        {
            _deviceName = deviceName;
            ManagerJoystickIndex = managerJoystickIndex;
        }

        public override string DeviceName => _deviceName;
        string _deviceName;

        public override bool RuntimeOnly => true;

        public override Dictionary<string, float> Values => _buttons;

        public Vector2 LeftTriggerDeadZone { get; set; } = new Vector2(0.1f, 0.9f);

        public int ManagerJoystickIndex { get; set; }


        private Dictionary<string, float> _buttons = new Dictionary<string, float>();
        private Dictionary<string, float> _buttonsUp = new Dictionary<string, float>();
        private Dictionary<string, float> _buttonsDown = new Dictionary<string, float>();

        public override float GetInputValue(string keyPath)
        {
            if (!_buttons.ContainsKey(keyPath))
                return 0f;

            return _buttons[keyPath];
        }

        public override InputEventType GetInputEvent(string keyPath)
        {
            InputEventType type = InputEventType.None;

            if (!_buttons.ContainsKey(keyPath))
                return type;

            if (_buttons[keyPath] > 0.5f)
                type = InputEventType.Pressed;

            if (_buttonsUp[keyPath] != 0)
                type = InputEventType.Up;

            if (_buttonsDown[keyPath] != 0)
                type = InputEventType.Down;

            return type;
        }

        public override string GetAnyKeyDown()
        {
            var downButtons = _buttonsDown
                .Where(x => x.Value != 0);

            return downButtons.FirstOrDefault().Key;
        }

        public override void Initialize()
        {
            _buttons.Clear();
            _buttonsUp.Clear();
            _buttonsDown.Clear();

            GamepadButton[] buttons = InputManager.GamepadButtons;
            foreach (var button in buttons)
            {
                string path = GetKeyPath(button);
                _buttons.Add(path, 0f);
                _buttonsUp.Add(path, 0f);
                _buttonsDown.Add(path, 0f);
            }
        }

        public override void Update()
        {
            foreach (var button in InputManager.GamepadButtons)
            {
                string path = GetKeyPath(button);
                float value = GetButtonValue(button);
                float previousValue = _buttons[path];

                _buttonsUp[path] = previousValue != 0f && value == 0f ? 1f : 0f;
                _buttonsDown[path] = previousValue == 0f && value != 0f ? 1f : 0f;
                _buttons[path] = value;
            }
        }

        float GetButtonValue(GamepadButton button)
        {
            UIMAxisMapperPlatform.ButtonMapping mapping = UIMAxisMapper.GetButtonMapping(button);

            switch (mapping.type)
            {
                case UIMAxisMapperPlatform.UIMInputType.Axis:
                    float value = UnityEngine.Input.GetAxis(string.Format(mapping.axisName, ManagerJoystickIndex));

                    value = mapping.negative ? (Mathf.Clamp(value, -1f, 0f) * -1f) : Mathf.Clamp(value, 0f, 1f);

                    if (HasDeadZone(button))
                        value = GamepadUtility.CalculateDeadZone(value, LeftTriggerDeadZone.x, LeftTriggerDeadZone.y);

                    return value;
                case UIMAxisMapperPlatform.UIMInputType.Button:
                    //Get key for gamepad
                    KeyCode key = (KeyCode)((int)mapping.uimKey + ManagerJoystickIndex * 20f);

                    return UnityEngine.Input.GetKey(key) ? 1f : 0f;
                default:
                    return 0f;
            }
        }

        bool HasDeadZone(GamepadButton button)
        {
            switch (button)
            {
                case GamepadButton.LeftStickUp:
                case GamepadButton.LeftStickRight:
                case GamepadButton.LeftStickDown:
                case GamepadButton.LeftStickLeft:
                case GamepadButton.RightStickUp:
                case GamepadButton.RightStickRight:
                case GamepadButton.RightStickDown:
                case GamepadButton.RightStickLeft:
                    return true;
                default:
                    return false;
            }
        }

        string GetKeyPath(GamepadButton button) =>
            $"key_gamepad/{button}";

        public override PropertiesList GetProperties()
        {
            var properties = base.GetProperties();

            properties.Add("Deadzone.Min", LeftTriggerDeadZone.x);
            properties.Add("Deadzone.Max", LeftTriggerDeadZone.y);

            return properties;
        }
    }
}
