using UnityEngine;
using qASIC.Console;

namespace qASIC
{
    public static class qDebug
    {
        public static void Log(string message) => GameConsoleController.Log(message, "default");
        public static void Log(string message, string color) => GameConsoleController.Log(message, color);
        public static void Log(string message, Color color) => GameConsoleController.Log(message, color);

        public static void LogWarning(string message) => GameConsoleController.Log(message, "warning");
        public static void LogError(string message) => GameConsoleController.Log(message, "error");
    }
}