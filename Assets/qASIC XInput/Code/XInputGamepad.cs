using qASIC.Input;
using qASIC.Input.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XInputDotNetPure;

namespace qASIC.XInput.Devices
{
    public class XInputGamepad : GamepadDevice, IDeadZone
    {
        public XInputGamepad() { }

        public XInputGamepad(string deviceName)
        {
            _deviceName = deviceName;
        }

        public XInputGamepad(string deviceName, PlayerIndex playerIndex)
        {
            _deviceName = deviceName;
            PlayerIndex = playerIndex;
        }

        public override string DeviceName => _deviceName;
        string _deviceName;

        public override bool RuntimeOnly => false;

        public override Dictionary<string, float> Values => _buttons;

        public Vector2 DeadZone { get; set; } = new Vector2(0.1f, 0.9f);

        public PlayerIndex PlayerIndex { get; set; }

        private GamePadState _state;

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
            _state = GamePad.GetState(PlayerIndex);

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
                    value = _state.Buttons.A == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.B:
                    value = _state.Buttons.B == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.X:
                    value = _state.Buttons.X == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.Y:
                    value = _state.Buttons.Y == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.LeftBumper:
                    value = _state.Buttons.LeftShoulder == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.RightBumper:
                    value = _state.Buttons.RightShoulder == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.LeftTrigger:
                    value = _state.Triggers.Left;
                    break;
                case GamepadButton.RightTrigger:
                    value = _state.Triggers.Right;
                    break;
                case GamepadButton.LeftStickButton:
                    value = _state.Buttons.LeftStick == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.RightStickButton:
                    value = _state.Buttons.RightStick == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.LeftStickUp:
                    value = Mathf.Clamp(_state.ThumbSticks.Left.Y, 0f, 1f);
                    break;
                case GamepadButton.LeftStickRight:
                    value = Mathf.Clamp(_state.ThumbSticks.Left.X, 0f, 1f);
                    break;
                case GamepadButton.LeftStickDown:
                    value = Mathf.Clamp(_state.ThumbSticks.Left.Y, -1f, 0f);
                    break;
                case GamepadButton.LeftStickLeft:
                    value = Mathf.Clamp(_state.ThumbSticks.Left.X, -1f, 0f);
                    break;
                case GamepadButton.RightStickUp:
                    value = Mathf.Clamp(_state.ThumbSticks.Right.Y, 0f, 1f);
                    break;
                case GamepadButton.RightStickRight:
                    value = Mathf.Clamp(_state.ThumbSticks.Right.X, 0f, 1f);
                    break;
                case GamepadButton.RightStickDown:
                    value = Mathf.Clamp(_state.ThumbSticks.Right.Y, -1f, 0f);
                    break;
                case GamepadButton.RightStickLeft:
                    value = Mathf.Clamp(_state.ThumbSticks.Right.X, -1f, 0f);
                    break;
                case GamepadButton.DPadUp:
                    value = _state.DPad.Up == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.DPadRight:
                    value = _state.DPad.Right == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.DPadDown:
                    value = _state.DPad.Down == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.DPadLeft:
                    value = _state.DPad.Left == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.Back:
                    value = _state.Buttons.Back == ButtonState.Pressed ? 1f : 0f;
                    break;
                case GamepadButton.Start:
                    value = _state.Buttons.Start == ButtonState.Pressed ? 1f : 0f;
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