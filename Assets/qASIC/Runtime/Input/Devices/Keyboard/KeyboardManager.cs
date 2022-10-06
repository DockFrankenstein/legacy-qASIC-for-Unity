using UnityEngine;
using System;
using System.Linq;

namespace qASIC.Input.Devices
{
    public static class KeyboardManager
    {
        private static KeyCode[] _allKeyCodes = null;
        /// <summary>A list of all existing KeyCodes</summary>
        public static KeyCode[] AllKeyCodes
        {
            get
            {
                if (_allKeyCodes == null)
                    _allKeyCodes = ((KeyCode[])Enum.GetValues(typeof(KeyCode))).Distinct().ToArray();

                return _allKeyCodes;
            }
        }

        private static UIMKeyboardDevice keyboard = new UIMKeyboardDevice();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Initialize()
        {
            DeviceManager.RegisterDevice(keyboard);
        }
    }
}