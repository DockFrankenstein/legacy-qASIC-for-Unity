using qASIC.Input.Devices;
using System.Collections.Generic;
using XInputDotNetPure;
using System;

namespace qASIC.XInput.Devices
{
    [Serializable]
    public class XInputGamepadProvider : DeviceProvider
    {
        const int DEVICE_LIMIT = 4;

        static readonly PlayerIndex[] _avaliableIndexes = new PlayerIndex[DEVICE_LIMIT]
        {
            PlayerIndex.One,
            PlayerIndex.Two,
            PlayerIndex.Three,
            PlayerIndex.Four,
        };


        static bool[] _slotConnectionStates = new bool[DEVICE_LIMIT];

        static List<XInputGamepad> _gamepads = new List<XInputGamepad>(new XInputGamepad[DEVICE_LIMIT]);

        public override string DefaultItemName => "XInput Gamepad Provider";

        public override void Update()
        {
            for (int i = 0; i < DEVICE_LIMIT; i++)
            {
                PlayerIndex index = _avaliableIndexes[i];
                bool isConnected = GamePad.GetState(index).IsConnected;

                //Device connected
                if (isConnected && !_slotConnectionStates[i])
                    OnDeviceConnected(index);

                //Device disconnected
                if (!isConnected && _slotConnectionStates[i])
                    OnDeviceDisconnected(index);

                _slotConnectionStates[i] = isConnected;
            }
        }

        public override void Cleanup()
        {
            _gamepads = new List<XInputGamepad>(new XInputGamepad[DEVICE_LIMIT]);
            _slotConnectionStates = new bool[DEVICE_LIMIT];
        }

        static void OnDeviceConnected(PlayerIndex index)
        {
            _slotConnectionStates[Array.IndexOf(_avaliableIndexes, index)] = true;
            XInputGamepad gamepad = new XInputGamepad($"Gamepad {index}", index);
            _gamepads[(int)index] = gamepad;
            DeviceManager.RegisterDevice(gamepad);
            qDebug.Log($"[XInput] Device connected: {gamepad.DeviceName}", "xinput");
        }

        static void OnDeviceDisconnected(PlayerIndex index)
        {
            _slotConnectionStates[Array.IndexOf(_avaliableIndexes, index)] = false;
            XInputGamepad gamepad = _gamepads[(int)index];
            DeviceManager.DeregisterDevice(gamepad);
            qDebug.Log($"[XInput] Device disconnected: {gamepad.DeviceName}", "xinput");
            _gamepads[(int)index] = null;
        }
    }
}