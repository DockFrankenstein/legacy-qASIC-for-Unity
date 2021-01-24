using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using qASIC.Console;

namespace qASIC
{
    public class Options
    {
        public static void ChangeResolution(int X, int Y)
        {
            Screen.SetResolution(X, Y, Screen.fullScreen);
            GameConsoleController.Log($"Changed resolution to: {X}x{Y}", "options");
        }

        public static void ChangeFullScreenMode(bool state)
        {
            Screen.fullScreen = state;
            GameConsoleController.Log($"Changed fullscreen to: {state}", "options");
        }

        public static void ChangeAntiAliasing(int value)
        {
            QualitySettings.antiAliasing = value;
            GameConsoleController.Log($"Changed antialiasing to: {value}", "options");
        }

        public static void ChangeFrameOptions(int vsync, int framelock)
        {
            QualitySettings.vSyncCount = vsync;
            Application.targetFrameRate = framelock;
            GameConsoleController.Log($"Changed vsync to: { vsync}", "options");
            GameConsoleController.Log($"Changed framelock to: {framelock}", "options");
        }
    }
}