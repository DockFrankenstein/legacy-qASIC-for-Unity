using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace qASIC.Input.Devices
{
    public class XInputGamepad : GamepadDevice, IStickDeadZone, ITriggerDeadZone
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

        enum XInputButton : ushort
        {
            None = 0x0000,
            DPadUp = 0x0001,
            DPadDown = 0x0002,
            DPadLeft = 0x0004,
            DPadRight = 0x0008,
            Start = 0x0010,
            Back = 0x0020,
            LeftStickButton = 0x0040,
            RightStickButton = 0x0080,
            LeftBumper = 0x0100,
            RightBumper = 0x0200,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000,
        }

        public override string DeviceName => _deviceName;
        string _deviceName;
        public uint PlayerIndex { get; set; }
        public override bool RuntimeOnly => false;

        //Deadzones
        public Vector2 LeftStickDeadZone { get; set; }
        public Vector2 RightStickDeadZone { get; set; }
        public Vector2 LeftTriggerDeadZone { get; set; }
        public Vector2 RightTriggerDeadZone { get; set; }

        private Dictionary<string, float> _buttons = new Dictionary<string, float>();
        private Dictionary<string, float> _buttonsUp = new Dictionary<string, float>();
        private Dictionary<string, float> _buttonsDown = new Dictionary<string, float>();
        public override Dictionary<string, float> Values => _buttons;

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
            XInputGetState(PlayerIndex, out XInputGamepadState state);

            var previousButtons = new Dictionary<string, float>(_buttons);

            _buttons["key_gamepad/A"] = XInputIsButtonPressed(state, XInputButton.A);
            _buttons["key_gamepad/B"] = XInputIsButtonPressed(state, XInputButton.B);
            _buttons["key_gamepad/X"] = XInputIsButtonPressed(state, XInputButton.X);
            _buttons["key_gamepad/Y"] = XInputIsButtonPressed(state, XInputButton.Y);

            _buttons["key_gamepad/LeftBumper"] = XInputIsButtonPressed(state, XInputButton.LeftBumper);
            _buttons["key_gamepad/RightBumper"] = XInputIsButtonPressed(state, XInputButton.RightBumper);

            _buttons["key_gamepad/LeftTrigger"] = GamepadUtility.CalculateDeadZone((float)state.leftTrigger / byte.MaxValue, LeftTriggerDeadZone.x, LeftTriggerDeadZone.y);
            _buttons["key_gamepad/RightTrigger"] = GamepadUtility.CalculateDeadZone((float)state.rightTrigger / byte.MaxValue, RightTriggerDeadZone.x, RightTriggerDeadZone.y);

            _buttons["key_gamepad/LeftStickButton"] = XInputIsButtonPressed(state, XInputButton.LeftStickButton);
            _buttons["key_gamepad/RightStickButton"] = XInputIsButtonPressed(state, XInputButton.RightStickButton);

            float leftStickX = GamepadUtility.CalculateDeadZone((float)state.leftStickX / short.MaxValue, LeftStickDeadZone.x, LeftStickDeadZone.y);
            float leftStickY = GamepadUtility.CalculateDeadZone((float)state.leftStickY / short.MaxValue, LeftStickDeadZone.x, LeftStickDeadZone.y);
            float rightStickX = GamepadUtility.CalculateDeadZone((float)state.rightStickX / short.MaxValue, RightStickDeadZone.x, RightStickDeadZone.y);
            float rightStickY = GamepadUtility.CalculateDeadZone((float)state.rightStickY / short.MaxValue, RightStickDeadZone.x, RightStickDeadZone.y);

            _buttons["key_gamepad/LeftStickUp"] = Mathf.Clamp(leftStickY, 0f, 1f);
            _buttons["key_gamepad/LeftStickRight"] = Mathf.Clamp(leftStickX, 0f, 1f);
            _buttons["key_gamepad/LeftStickDown"] = -Mathf.Clamp(leftStickY, -1f, 0f);
            _buttons["key_gamepad/LeftStickLeft"] = -Mathf.Clamp(leftStickX, -1f, 0f);

            _buttons["key_gamepad/RightStickUp"] = Mathf.Clamp(rightStickY, 0f, 1f);
            _buttons["key_gamepad/RightStickRight"] = Mathf.Clamp(rightStickX, 0f, 1f);
            _buttons["key_gamepad/RightStickDown"] = -Mathf.Clamp(rightStickY, -1f, 0f);
            _buttons["key_gamepad/RightStickLeft"] = -Mathf.Clamp(rightStickX, -1f, 0f);

            _buttons["key_gamepad/DPadUp"] = XInputIsButtonPressed(state, XInputButton.DPadUp);
            _buttons["key_gamepad/DPadRight"] = XInputIsButtonPressed(state, XInputButton.DPadRight);
            _buttons["key_gamepad/DPadDown"] = XInputIsButtonPressed(state, XInputButton.DPadDown);
            _buttons["key_gamepad/DPadLeft"] = XInputIsButtonPressed(state, XInputButton.DPadLeft);

            _buttons["key_gamepad/Back"] = XInputIsButtonPressed(state, XInputButton.Back);
            _buttons["key_gamepad/Start"] = XInputIsButtonPressed(state, XInputButton.Start);

            foreach (var item in _buttons)
            {
                _buttonsUp[item.Key] = previousButtons[item.Key] != 0f && _buttons[item.Key] == 0f ? 1f : 0f;
                _buttonsDown[item.Key] = previousButtons[item.Key] == 0f && _buttons[item.Key] != 0f ? 1f : 0f;
            }
        }

        string GetKeyPath(GamepadButton button) =>
            $"key_gamepad/{button}";

        #region XInput
        public static bool IsPlayerConnected(uint playerIndex) =>
            XInputGetState(playerIndex, out _) == 0;

        static float XInputIsButtonPressed(XInputGamepadState state, XInputButton button)
        {
            return ((state.buttons & (ushort)button) == 0) ? 0f : 1f;
        }

        [DllImport("XINPUT9_1_0.DLL")]
        private static extern uint XInputGetState(uint playerIndex, out XInputGamepadState state);

        [DllImport("XINPUT9_1_0.DLL")]
        private static extern uint XInputSetState(uint playerIndex, ref XInputVibrationData data);

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputGamepadState
        {
            public uint packetNumber;
            public ushort buttons;
            public byte leftTrigger;
            public byte rightTrigger;
            public short leftStickX;
            public short leftStickY;
            public short rightStickX;
            public short rightStickY;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputVibrationData
        {
            public ushort LeftMotorSpeed;
            public ushort RightMotorSpeed;
        }
        #endregion
    }
}