using UnityEngine;
using qASIC.Console;
using qASIC.Displayer;
using qASIC.ProjectSettings;
using System;
using qASIC.Internal;

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


        #region Internal
        internal static void LogInternal(object message) =>
            GameConsoleController.Log(GameConsoleLog.CreateNow(message?.ToString() ?? "null", "default", GameConsoleLog.LogType.Internal, true));

        internal static void LogInternal(object message, string color) =>
            GameConsoleController.Log(GameConsoleLog.CreateNow(message?.ToString() ?? "null", color, GameConsoleLog.LogType.Internal, true));

        internal static void LogInternal(object message, Color color) =>
            GameConsoleController.Log(GameConsoleLog.CreateNow(message?.ToString() ?? "null", color, GameConsoleLog.LogType.Internal, true));


        internal static void LogWarningInternal(object message) =>
            GameConsoleController.Log(GameConsoleLog.CreateNow(message?.ToString() ?? "null", "warning", GameConsoleLog.LogType.Internal, true));

        internal static void LogErrorInternal(object message) =>
            GameConsoleController.Log(GameConsoleLog.CreateNow(message?.ToString() ?? "null", "error", GameConsoleLog.LogType.Internal, true));
        #endregion

        public static void DisplayValue(string tag, object value)
        {
            DisplayerProjectSettings settings = DisplayerProjectSettings.Instance;
            if (settings.CreateDebugDisplayer && !InfoDisplayer.DisplayerExists(settings.debugDisplayerName))
            {
                qASICObjectCreator.CreateDebugDisplyer();
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
                qASICObjectCreator.CreateDebugDisplyer();
                if (settings.displayDebugGenerationMessage)
                    GameConsoleController.Log(settings.debugGenerationMessage, settings.debugGenerationMessageColor);
            }

            InfoDisplayer.ToggleValue(tag, show, settings.debugDisplayerName);
        }
    }
}