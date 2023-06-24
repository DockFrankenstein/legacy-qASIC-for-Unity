using UnityEngine;

using static qASIC.Internal.qLogger;

namespace qASIC
{
    public static partial class qDebug
    {
        public static void Log(object message) =>
            OnLogColorTag?.Invoke(message?.ToString() ?? "null", "default");

        public static void Log(object message, string color) =>
            OnLogColorTag?.Invoke(message?.ToString() ?? "null", color);

        public static void Log(object message, Color color) =>
            OnLogColor?.Invoke(message?.ToString() ?? "null", color);


        public static void LogWarning(object message) =>
            OnLogColorTag?.Invoke(message?.ToString() ?? "null", "warning");

        public static void LogError(object message) =>
            OnLogColorTag?.Invoke(message?.ToString() ?? "null", "error");


        #region Internal
        public static void LogInternal(object message) =>
            OnLogColorTagInternal?.Invoke(message?.ToString() ?? "null", "default");

        public static void LogInternal(object message, string color) =>
            OnLogColorTagInternal?.Invoke(message?.ToString() ?? "null", color);

        public static void LogInternal(object message, Color color) =>
            OnLogColorInternal?.Invoke(message?.ToString() ?? "null", color);


        public static void LogWarningInternal(object message) =>
            OnLogColorTagInternal?.Invoke(message?.ToString() ?? "null", "warning");

        public static void LogErrorInternal(object message) =>
            OnLogColorTagInternal?.Invoke(message?.ToString() ?? "null", "error");
        #endregion
    }
}