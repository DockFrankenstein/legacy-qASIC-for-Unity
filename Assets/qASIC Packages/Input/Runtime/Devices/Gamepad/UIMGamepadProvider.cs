using System.Collections.Generic;
using System.Linq;
using qASIC.Input.UIM;
using UnityEngine;

using UInput = UnityEngine.Input;

namespace qASIC.Input.Devices
{
    [System.Serializable]
    public class UIMGamepadProvider : DeviceProvider
    {
        public UIMAxisMapper mapper;
        public Vector2 deadzone;

        static string[] _joystickNames;

        static List<UIMGamepad> _gamepads = new List<UIMGamepad>();

        public override string DefaultItemName => "UIM Gamepad Provider";
        public static UIMAxisMapper AxisMapper { get; private set; } = null;

        public override void Initialize()
        {
            AxisMapper = mapper;
            _joystickNames = UInput.GetJoystickNames();

            for (int i = 0; i < _joystickNames.Length; i++)
                if (!string.IsNullOrEmpty(_joystickNames[i]))
                    AddGamepad(_joystickNames[i], i);
        }

        public override void Cleanup()
        {
            AxisMapper = null;
            _gamepads.Clear();
            _joystickNames = new string[0];
        }

        public override void Update()
        {
            string[] joysticks = UInput.GetJoystickNames();

            //Return if nothing changed
            if (joysticks.SequenceEqual(_joystickNames))
                return;

            for (int i = 0; i < joysticks.Length; i++)
            {
                //More items than previously
                if (_joystickNames.Length <= i && !string.IsNullOrEmpty(joysticks[i]))
                {
                    AddGamepad(joysticks[i], i);
                    continue;
                }

                //Something changed
                if (joysticks[i] != _joystickNames[i])
                {
                    switch (string.IsNullOrEmpty(joysticks[i]))
                    {
                        //Device disconnected
                        case true:
                            RemoveGamepad(i);
                            break;
                        //Device connected
                        case false:
                            AddGamepad(joysticks[i], i);
                            break;
                    }
                }
            }

            _joystickNames = joysticks;
        }

        private static void AddGamepad(string name, int id)
        {
            UIMGamepad gamepad = new UIMGamepad(name, id);
            UpdateGamepadList();
            _gamepads[id] = gamepad;
            DeviceManager.RegisterDevice(gamepad);
        }

        private static void UpdateGamepadList()
        {
            int gamepadCount = _gamepads.Count;
            int joystickCount = UInput.GetJoystickNames().Length;

            //Do nothing if the count is the same
            if (gamepadCount == joystickCount) return;

            //If new joysticks got registered
            if (gamepadCount < joystickCount)
            {
                _gamepads.AddRange(new UIMGamepad[joystickCount - gamepadCount]);
                return;
            }

            //If joysticks got removed (which is impossible, but we do it
            //just in case if that would suddenly change to save on performence)
            if (gamepadCount > joystickCount)
            {
                _gamepads.RemoveRange(joystickCount - 1, gamepadCount - joystickCount);
                return;
            }
        }

        private static void RemoveGamepad(int id)
        {
            if (_gamepads[id] == null)
                return;

            DeviceManager.DeregisterDevice(_gamepads[id]);
            _gamepads[id] = null;
        }
    }
}