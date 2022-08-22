using System;
using System.Collections.Generic;
using qASIC.InputManagement.UIM;
using qASIC.ProjectSettings;
using UnityEngine;

namespace qASIC.InputManagement.Devices
{
    public class UIMGamepad : IGamepadDevice
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

        public string DeviceName => _deviceName;
        string _deviceName;
        public Type KeyType => typeof(GamepadButton);

        public float DeadZoneInner { get; set; }
        public float DeadZoneOuter { get; set; }
        public int ManagerJoystickIndex { get; set; }


        private Dictionary<GamepadButton, float> _buttons = new Dictionary<GamepadButton, float>();
        private Dictionary<GamepadButton, float> _buttonsUp = new Dictionary<GamepadButton, float>();
        private Dictionary<GamepadButton, float> _buttonsDown = new Dictionary<GamepadButton, float>();

        public void SetName(string name)
        {
            _deviceName = name;
        }

        public float GetInputValue(int keyIndex)
        {
            GamepadButton key = (GamepadButton)keyIndex;
            if (!_buttons.ContainsKey(key))
                return 0f;

            return _buttons[key];
        }

        public bool GetInputEvent(KeyEventType type, int keyIndex)
        {
            GamepadButton key = (GamepadButton)keyIndex;
            Dictionary<GamepadButton, float> dictionary = _buttons;
            switch (type)
            {
                case KeyEventType.up:
                    dictionary = _buttonsUp;
                    break;
                case KeyEventType.down:
                    dictionary = _buttonsDown;
                    break;
            }

            if (!dictionary.ContainsKey(key))
                return false;

            return dictionary[key] >= 0.5f;
        }

        public void Initialize()
        {
            _buttons.Clear();
            _buttonsUp.Clear();
            _buttonsDown.Clear();

            GamepadButton[] buttons = InputManager.GamepadButtons;
            foreach (var button in buttons)
            {
                _buttons.Add(button, 0f);
                _buttonsUp.Add(button, 0f);
                _buttonsDown.Add(button, 0f);
            }
        }

        public void Update()
        {
            foreach (var button in InputManager.GamepadButtons)
            {
                float value = GetButtonValue(button);
                float previousValue = _buttons[button];

                _buttonsUp[button] = previousValue != 0f && value == 0f ? 1f : 0f;
                _buttonsDown[button] = previousValue == 0f && value != 0f ? 1f : 0f;
                _buttons[button] = value;
            }
        }

        float GetButtonValue(GamepadButton button)
        {
            UIMAxisMapper mapper = InputProjectSettings.Instance?.uimAxisMapper;
            if (mapper == null)
                return 0f;

            UIMAxisMapperPlatform.ButtonMapping mapping = UIMAxisMapper.GetButtonMapping(button);

            switch (mapping.type)
            {
                case UIMAxisMapperPlatform.UIMInputType.Axis:
                    float value = Input.GetAxis(string.Format(mapping.axisName, ManagerJoystickIndex));

                    value = mapping.negative ? (Mathf.Clamp(value, -1f, 0f) * -1f) : Mathf.Clamp(value, 0f, 1f);
                    return value;
                case UIMAxisMapperPlatform.UIMInputType.Button:
                    //Get key for gamepad
                    KeyCode key = (KeyCode)((int)mapping.uimKey + ManagerJoystickIndex * 20f);

                    return Input.GetKey(key) ? 1f : 0f;
            }

            return 0f;
        }
    }
}
