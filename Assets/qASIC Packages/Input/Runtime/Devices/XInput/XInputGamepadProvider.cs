using System.Collections.Generic;
using System;
using UnityEngine;

namespace qASIC.Input.Devices
{
    [Serializable]
    public class XInputGamepadProvider : DeviceProvider
    {
        const uint DEVICE_LIMIT = 4;

        static bool[] _slotConnectionStates = new bool[DEVICE_LIMIT];

        static List<XInputGamepad> _gamepads = new List<XInputGamepad>(new XInputGamepad[DEVICE_LIMIT]);

        [Header("Deadzones")]
        [InspectorLabel("Left Stick")] public Vector2 leftStickDeadzone = new Vector2(0.1f, 0.9f);
        [InspectorLabel("Right Stick")] public Vector2 rightStickDeadzone = new Vector2(0.1f, 0.9f);

        [Space]
        [InspectorLabel("Left Trigger")] public Vector2 leftTriggerDeadzone = new Vector2(0.05f, 1f);
        [InspectorLabel("Right Trigger")] public Vector2 rightTriggerDeadzone = new Vector2(0.05f, 1f);

        public override string DefaultItemName => "XInput Gamepad Provider";
        public override RuntimePlatformFlags SupportedPlatforms => RuntimePlatformFlags.WindowsPlayer |
            RuntimePlatformFlags.WindowsEditor;

        public override void Update()
        {
            for (uint i = 0; i < DEVICE_LIMIT; i++)
            {
                bool isConnected = XInputGamepad.IsPlayerConnected(i);

                //Device connected
                if (isConnected && !_slotConnectionStates[i])
                    OnDeviceConnected(i);

                //Device disconnected
                if (!isConnected && _slotConnectionStates[i])
                    OnDeviceDisconnected(i);

                _slotConnectionStates[i] = isConnected;
            }
        }

        public override void Cleanup()
        {
            _gamepads = new List<XInputGamepad>(new XInputGamepad[DEVICE_LIMIT]);
            _slotConnectionStates = new bool[DEVICE_LIMIT];
        }

        void OnDeviceConnected(uint index)
        {
            _slotConnectionStates[index] = true;
            XInputGamepad gamepad = new XInputGamepad($"Gamepad {index}", index)
            {
                LeftStickDeadZone = leftStickDeadzone,
                RightStickDeadZone = rightStickDeadzone,
                LeftTriggerDeadZone = leftTriggerDeadzone,
                RightTriggerDeadZone = rightTriggerDeadzone,
            };
            _gamepads[(int)index] = gamepad;
            DeviceManager.RegisterDevice(gamepad);
            qDebug.Log($"[XInput] Device connected: {gamepad.DeviceName}", "xinput");
        }

        void OnDeviceDisconnected(uint index)
        {
            _slotConnectionStates[index] = false;
            XInputGamepad gamepad = _gamepads[(int)index];
            DeviceManager.DeregisterDevice(gamepad);
            qDebug.Log($"[XInput] Device disconnected: {gamepad.DeviceName}", "xinput");
            _gamepads[(int)index] = null;
        }
    }
}