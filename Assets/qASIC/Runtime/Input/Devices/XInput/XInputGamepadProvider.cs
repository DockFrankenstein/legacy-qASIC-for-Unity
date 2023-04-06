using System.Collections.Generic;
using System;

namespace qASIC.Input.Devices
{
    [Serializable]
    public class XInputGamepadProvider : DeviceProvider
    {
        const uint DEVICE_LIMIT = 4;

        static bool[] _slotConnectionStates = new bool[DEVICE_LIMIT];

        static List<XInputGamepad> _gamepads = new List<XInputGamepad>(new XInputGamepad[DEVICE_LIMIT]);

        public override string DefaultItemName => "XInput Gamepad Provider";

        public override void Update()
        {
            for (uint i = 0; i < DEVICE_LIMIT; i++)
            {
                bool isConnected = XInput.IsControllerConnected(i);

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

        static void OnDeviceConnected(uint index)
        {
            _slotConnectionStates[index] = true;
            XInputGamepad gamepad = new XInputGamepad($"Gamepad {index}", index);
            _gamepads[(int)index] = gamepad;
            DeviceManager.RegisterDevice(gamepad);
            qDebug.Log($"[XInput] Device connected: {gamepad.DeviceName}", "xinput");
        }

        static void OnDeviceDisconnected(uint index)
        {
            _slotConnectionStates[index] = false;
            XInputGamepad gamepad = _gamepads[(int)index];
            DeviceManager.DeregisterDevice(gamepad);
            qDebug.Log($"[XInput] Device disconnected: {gamepad.DeviceName}", "xinput");
            _gamepads[(int)index] = null;
        }
    }
}