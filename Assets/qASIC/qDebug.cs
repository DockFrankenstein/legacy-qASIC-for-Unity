using UnityEngine;
using qASIC.Console;
using qASIC.Displayer;

namespace qASIC
{
    public static class qDebug
    {
        public static void Log(object message) =>
            GameConsoleController.Log(message == null ? "null" : message.ToString(), "default");

        public static void Log(object message, string color) =>
            GameConsoleController.Log(message == null ? "null" : message.ToString(), color);

        public static void Log(object message, Color color) =>
            GameConsoleController.Log(message == null ? "null" : message.ToString(), color);


        public static void LogWarning(object message) =>
            GameConsoleController.Log(message == null ? "null" : message.ToString(), "warning");

        public static void LogError(object message) =>
            GameConsoleController.Log(message == null ? "null" : message.ToString(), "error");

        public static void DisplayValue(string tag, object value)
        {
#if UNITY_EDITOR
            if (!InfoDisplayer.DisplayerExists("debug"))
            {
                Tools.qASICObjectCreator.CreateDebugDisplyer();
                GameConsoleController.Log("Generated debug displayer. This only happens in the editor!", "warning");
            }
#endif

            InfoDisplayer.DisplayValue(tag, value == null ? "null" : value.ToString(), "debug");
        }
    }
}