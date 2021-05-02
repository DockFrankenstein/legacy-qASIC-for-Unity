using UnityEngine;

namespace qASIC.Options
{
    public class BasicOptions
    {
        [OptionsSetting("resolution", typeof(string))]
        public static void ChangeResolution(string resolution)
        {
            Vector2Int res = VectorStringConvertion.StringToVector2Int(resolution);
            if (res == new Vector2Int(Screen.width, Screen.height)) return;
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
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
            Screen.fullScreenMode = state ? FullScreenMode.MaximizedWindow : FullScreenMode.Windowed;
        }

        [OptionsSetting("framelimit", typeof(int))]
        public static void ChangeFramerateLimit(int value)
        {
            if (value == Application.targetFrameRate) return;
            Application.targetFrameRate = value;
        }

        [OptionsSetting("vsync", typeof(bool))]
        public static void ChangeVSync(bool value)
        {
            int intValue = value ? 1 : 0;
            if (intValue == QualitySettings.vSyncCount) return;
            QualitySettings.vSyncCount = intValue;
        }
    }
}