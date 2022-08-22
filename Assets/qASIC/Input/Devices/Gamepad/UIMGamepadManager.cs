using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace qASIC.InputManagement.Devices
{
    public static class UIMGamepadManager
    {
        static string[] _joystickNames;

        static List<UIMGamepad> _gamepads = new List<UIMGamepad>();

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            InputUpdateManager.OnUpdate += Update;

            _joystickNames = Input.GetJoystickNames();

            for (int i = 0; i < _joystickNames.Length; i++)
                if (!string.IsNullOrEmpty(_joystickNames[i]))
                    AddGamepad(_joystickNames[i], i);
        }

        private static void Update()
        {
            string[] joysticks = Input.GetJoystickNames();

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
            int joystickCount = Input.GetJoystickNames().Length;

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
            DeviceManager.DeregisterDevice(_gamepads[id]);
            _gamepads[id] = null;
        }
    }
}