using UnityEngine;
using System;

namespace qASIC.InputManagement
{
    public static class KeyboardManager
    {
        /// <summary>A list of all existing KeyCodes</summary>
        public static KeyCode[] AllKeyCodes { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Initialize()
        {
            AllKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        }
    }
}