using UnityEngine;

namespace qASIC.Options
{
    public class BasicOptions
    {
        [OptionsSetting("resolution", defaultValueMethodName = nameof(GetDefaultResolution))]
        public static void ChangeResolution(string resolution)
        {
            Vector2Int res = VectorText.ToVector2Int(resolution);
            Screen.SetResolution(res.x, res.y, Screen.fullScreen);
        }

        public static string GetDefaultResolution() =>
            VectorText.ToText(new Vector2(Screen.width, Screen.height));


        [OptionsSetting("fullscreen", defaultValueMethodName = nameof(GetDefaultFullScreenMode))]
        public static void ChangeFullScreenMode(FullScreenMode state) =>
            Screen.fullScreenMode = state;

        [OptionsSetting("fullscreen", defaultValueMethodName = nameof(GetDefaultFullScreenState))]
        public static void ChangeFullScreen(bool state) =>
            Screen.fullScreen = state;

        public static FullScreenMode GetDefaultFullScreenMode() =>
            Screen.fullScreenMode;

        public static bool GetDefaultFullScreenState() =>
            Screen.fullScreen;


        [OptionsSetting("framelimit", defaultValueMethodName = nameof(GetDefaultFrameLimit))]
        public static void ChangeFramerateLimit(int value) =>
            Application.targetFrameRate = value;

        public static int GetDefaultFrameLimit() =>
            Application.targetFrameRate;


        [OptionsSetting("vsync", defaultValueMethodName = nameof(GetDefaultVSync))]
        public static void ChangeVSync(bool value)
        {
            int intValue = value ? 1 : 0;
            QualitySettings.vSyncCount = intValue;
        }

        [OptionsSetting("vsync", defaultValueMethodName = nameof(GetDefaultVSync))]
        public static void ChangeVSync(int value) =>
            QualitySettings.vSyncCount = value;

        public static int GetDefaultVSync() =>
            QualitySettings.vSyncCount;
    }
}