//This is a very big task that will not be finished for the next update

/*using System;
using UnityEngine;

namespace qASIC.InputManagement
{
    public static class GamepadManager
    {
        public static bool GetGamepadButton(GamepadButton key, int gamepadIndex = 0) =>
            HandleGamepadButton(new Func<KeyCode, bool>((KeyCode keyCode) => { return Input.GetKey(keyCode); }), key, gamepadIndex);
        public static bool GetGamepadButtonDown(GamepadButton key, int gamepadIndex = 0) =>
            HandleGamepadButton(new Func<KeyCode, bool>((KeyCode keyCode) => { return Input.GetKeyDown(keyCode); }), key, gamepadIndex);
        public static bool GetGamepadButtonUp(GamepadButton key, int gamepadIndex = 0) =>
            HandleGamepadButton(new Func<KeyCode, bool>((KeyCode keyCode) => { return Input.GetKeyUp(keyCode); }), key, gamepadIndex);

        public static bool HandleGamepadButton(Func<KeyCode, bool> statement, GamepadButton key, int gamepadIndex = 0)
        {
            KeyCode keyCode;
            int keyIndex = (int)key;

            switch(Application.platform)
            {
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    keyIndex = GetMacGamepadButton(keyIndex);
                    break;
            }

            for (int i = 0; i < gamepadIndex; i++)
                keyIndex += 20;

            keyCode = (KeyCode)keyIndex;
            return statement.Invoke(keyCode);
        }

        public static int GetMacGamepadButton(int buttonIndex)
        {
            switch(buttonIndex)
            {
                case 330:
                    return 346;
                case 331:
                    return 347;
                case 332:
                    return 348;
                case 333:
                    return 349;
                case 334:
                    return 343;
                case 335:
                    return 344;
                case 336:
                    return 340;
                case 337:
                    return 339;
                case 338:
                    return 341;
                case 339:
                    return 342;
                default:
                    return 0;
            }
        }
    }
}*/