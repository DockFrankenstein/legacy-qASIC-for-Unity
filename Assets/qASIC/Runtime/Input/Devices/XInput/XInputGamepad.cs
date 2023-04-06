using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace qASIC.Input.Devices
{
    public class XInputGamepad : GamepadDevice, IDeadZone
    {
        public XInputGamepad() { }

        public XInputGamepad(string deviceName)
        {
            _deviceName = deviceName;
        }

        public XInputGamepad(string deviceName, uint playerIndex)
        {
            _deviceName = deviceName;
            PlayerIndex = playerIndex;
        }

        public override string DeviceName => _deviceName;
        string _deviceName;

        public override bool RuntimeOnly => false;

        public override Dictionary<string, float> Values => _buttons;

        public Vector2 DeadZone { get; set; } = new Vector2(0.1f, 0.9f);

        public uint PlayerIndex { get; set; }

        private Dictionary<string, float> _buttons = new Dictionary<string, float>();
        private Dictionary<string, float> _buttonsUp = new Dictionary<string, float>();
        private Dictionary<string, float> _buttonsDown = new Dictionary<string, float>();

        public void SetName(string name)
        {
            _deviceName = name;
        }

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
            float value;

            switch (button)
            {
                case GamepadButton.A:
                    value = XInput.GetButton(PlayerIndex, XInputButton.A) ? 1f : 0f;
                    break;
                case GamepadButton.B:
                    value = XInput.GetButton(PlayerIndex, XInputButton.B) ? 1f : 0f;
                    break;
                case GamepadButton.X:
                    value = XInput.GetButton(PlayerIndex, XInputButton.X) ? 1f : 0f;
                    break;
                case GamepadButton.Y:
                    value = XInput.GetButton(PlayerIndex, XInputButton.Y) ? 1f : 0f;
                    break;
                case GamepadButton.LeftBumper:
                    value = XInput.GetButton(PlayerIndex, XInputButton.LeftShoulder) ? 1f : 0f;
                    break;
                case GamepadButton.RightBumper:
                    value = XInput.GetButton(PlayerIndex, XInputButton.RightShoulder) ? 1f : 0f;
                    break;
                case GamepadButton.LeftTrigger:
                    value = XInput.GetTriggerLeft(PlayerIndex);
                    break;
                case GamepadButton.RightTrigger:
                    value = XInput.GetTriggerRight(PlayerIndex);
                    break;
                case GamepadButton.LeftStickButton:
                    value = XInput.GetButton(PlayerIndex, XInputButton.LeftThumb) ? 1f : 0f;
                    break;
                case GamepadButton.RightStickButton:
                    value = XInput.GetButton(PlayerIndex, XInputButton.RightThumb) ? 1f : 0f;
                    break;
                case GamepadButton.LeftStickUp:
                    value = Mathf.Clamp(XInput.GetThumbStickLeft(PlayerIndex).y, 0f, 1f);
                    break;
                case GamepadButton.LeftStickRight:
                    value = Mathf.Clamp(XInput.GetThumbStickLeft(PlayerIndex).x, 0f, 1f);
                    break;
                case GamepadButton.LeftStickDown:
                    value = Mathf.Clamp(XInput.GetThumbStickLeft(PlayerIndex).y, -1f, 0f);
                    break;
                case GamepadButton.LeftStickLeft:
                    value = Mathf.Clamp(XInput.GetThumbStickLeft(PlayerIndex).x, -1f, 0f);
                    break;
                case GamepadButton.RightStickUp:
                    value = Mathf.Clamp(XInput.GetThumbStickRight(PlayerIndex).y, 0f, 1f);
                    break;
                case GamepadButton.RightStickRight:
                    value = Mathf.Clamp(XInput.GetThumbStickRight(PlayerIndex).x, 0f, 1f);
                    break;
                case GamepadButton.RightStickDown:
                    value = Mathf.Clamp(XInput.GetThumbStickRight(PlayerIndex).y, -1f, 0f);
                    break;
                case GamepadButton.RightStickLeft:
                    value = Mathf.Clamp(XInput.GetThumbStickRight(PlayerIndex).x, -1f, 0f);
                    break;
                case GamepadButton.DPadUp:
                    value = XInput.GetButton(PlayerIndex, XInputButton.DPadUp) ? 1f : 0f;
                    break;
                case GamepadButton.DPadRight:
                    value = XInput.GetButton(PlayerIndex, XInputButton.DPadRight) ? 1f : 0f;
                    break;
                case GamepadButton.DPadDown:
                    value = XInput.GetButton(PlayerIndex, XInputButton.DPadDown) ? 1f : 0f;
                    break;
                case GamepadButton.DPadLeft:
                    value = XInput.GetButton(PlayerIndex, XInputButton.DPadLeft) ? 1f : 0f;
                    break;
                case GamepadButton.Back:
                    value = XInput.GetButton(PlayerIndex, XInputButton.Back) ? 1f : 0f;
                    break;
                case GamepadButton.Start:
                    value = XInput.GetButton(PlayerIndex, XInputButton.Start) ? 1f : 0f;
                    break;
                default:
                    value = 0;
                    break;
            }

            if (HasDeadZone(button))
                value = GamepadUtility.CalculateDeadZone(value, DeadZone.x, DeadZone.y);

            return Mathf.Abs(value);
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
    }
}