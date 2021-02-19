using UnityEngine;
using qASIC.Console;

namespace qASIC.Options
{
    public class BasicOptions
    {
        [OptionsSetting("resolution", typeof(Vector2Int))]
        public static void ChangeResolution(Vector2Int resolution)
        {
            if (resolution == new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height)) return;
            Screen.SetResolution(resolution.x, resolution.y, Screen.fullScreen);
        }

        [OptionsSetting("fullscreen", typeof(FullScreenMode))]
        public static void ChangeFullScreenMode(FullScreenMode state)
        {
            if(state == Screen.fullScreenMode) return;
            Screen.fullScreenMode = state;
        }

        [OptionsSetting("fullscreen", typeof(bool))]
        public static void ChangeFullScreenMode(bool state)
        {
            if (state == Screen.fullScreen) return;
            Screen.fullScreen = state;
        }

        [OptionsSetting("framelimit", typeof(int))]
        public static void ChangeFramerateLimit(int value)
        {
            if (value == Application.targetFrameRate) return;
            Application.targetFrameRate = value;
        }

        [OptionsSetting("vsync", typeof(bool))]
        public static void ChangeVSync(bool state) => ChangeVSync(state ? 1 : 0);
        [OptionsSetting("vsync", typeof(int))]
        public static void ChangeVSync(int value)
        {
            if (value == QualitySettings.vSyncCount) return;
            QualitySettings.vSyncCount = value;
        }
    }
}