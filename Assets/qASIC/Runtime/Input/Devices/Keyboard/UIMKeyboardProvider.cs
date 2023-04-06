using System;
using UnityEngine;
using System.Linq;

namespace qASIC.Input.Devices
{
    [Serializable]
    public class UIMKeyboardProvider : DeviceProvider
    {
        private static KeyCode[] _allKeyCodes = null;
        /// <summary>A list of all existing KeyCodes</summary>
        public static KeyCode[] AllKeyCodes
        {
            get
            {
                if (_allKeyCodes == null)
                    _allKeyCodes = ((KeyCode[])Enum.GetValues(typeof(KeyCode)))
                        .Where(x => !(KeyCode.JoystickButton0 <= x && x <= KeyCode.Joystick8Button19))
                        .Distinct()
                        .ToArray();

                return _allKeyCodes;
            }
        }

        private static UIMKeyboardDevice keyboard = new UIMKeyboardDevice();

        public override string DefaultItemName => "UIM Keyboard Provider";

        public override void Initialize()
        {
            DeviceManager.RegisterDevice(keyboard);
        }
    }
}
 