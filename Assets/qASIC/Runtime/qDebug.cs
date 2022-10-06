using UnityEngine;
using qASIC.Console;
using qASIC.Displayer;
using qASIC.ProjectSettings;

namespace qASIC
{
    public static class qDebug
    {
        public static void Log(object message) =>
            GameConsoleController.Log(message?.ToString() ?? "null", "default");

        public static void Log(object message, string color) =>
            GameConsoleController.Log(message?.ToString() ?? "null", color);

        public static void Log(object message, Color color) =>
            GameConsoleController.Log(message?.ToString() ?? "null", color);


        public static void LogWarning(object message) =>
            GameConsoleController.Log(message?.ToString() ?? "null", "warning");

        public static void LogError(object message) =>
            GameConsoleController.Log(message?.ToString() ?? "null", "error");

        public static void DisplayValue(string tag, object value)
        {
            DisplayerProjectSettings settings = DisplayerProjectSettings.Instance;
            if (settings.CreateDebugDisplayer && !InfoDisplayer.DisplayerExists(settings.debugDisplayerName))
            {
                Tools.qASICObjectCreator.CreateDebugDisplyer();
                if (settings.displayDebugGenerationMessage)
                    GameConsoleController.Log(settings.debugGenerationMessage, settings.debugGenerationMessageColor);
            }

            InfoDisplayer.DisplayValue(tag, value?.ToString() ?? "null", settings.debugDisplayerName);
        }

        public static void ToggleDisplayValue(string tag, bool show)
        {
            DisplayerProjectSettings settings = DisplayerProjectSettings.Instance;
            if (settings.CreateDebugDisplayer && !InfoDisplayer.DisplayerExists(settings.debugDisplayerName))
            {
                Tools.qASICObjectCreator.CreateDebugDisplyer();
                if (settings.displayDebugGenerationMessage)
                    GameConsoleController.Log(settings.debugGenerationMessage, settings.debugGenerationMessageColor);
            }

            InfoDisplayer.ToggleValue(tag, show, settings.debugDisplayerName);
        }
    }
}