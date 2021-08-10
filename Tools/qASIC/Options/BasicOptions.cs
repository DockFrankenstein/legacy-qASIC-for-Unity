using UnityEngine;

namespace qASIC.Options
{
    public class BasicOptions
    {
        [OptionsSetting("resolution", typeof(string))]
        public static void ChangeResolution(string resolution)
        {
            Vector2Int res = VectorText.ToVector2Int(resolution);
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        }

        [OptionsSetting("fullscreen", typeof(FullScreenMode))]
        public static void ChangeFullScreenMode(FullScreenMode state) =>
            Screen.fullScreenMode = state;

        [OptionsSetting("fullscreen", typeof(bool))]
        public static void ChangeFullScreenMode(bool state) =>
            Screen.fullScreen = state;

        [OptionsSetting("framelimit", typeof(int))]
        public static void ChangeFramerateLimit(int value) =>
            Application.targetFrameRate = value;

        [OptionsSetting("vsync", typeof(bool))]
        public static void ChangeVSync(bool value)
        {
            int intValue = value ? 1 : 0;
            QualitySettings.vSyncCount = intValue;
        }

        [OptionsSetting("vsync", typeof(int))]
        public static void ChangeVSync(int value) =>
            QualitySettings.vSyncCount = value;
    }
}