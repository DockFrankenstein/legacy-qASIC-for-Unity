using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace qASIC.Console
{
    public class GameConsoleLog
    {
        public enum LogType { User, Game, Clear, Internal };
        public LogType Type { get; }
        public DateTime Time { get; }
        public string Message { get; }
        public Color LogColor { get; }
        public string ColorName { get; }
        
        /// <summary>If the log should be hidden in Unity console</summary>
        public bool UnityHidden { get; }

        public GameConsoleLog(string message, DateTime time, string color, LogType logType, bool unityHidden = false)
        {
            Type = logType;
            Time = time;
            Message = message ?? string.Empty;
            ColorName = color;
            UnityHidden = unityHidden;
        }

        public GameConsoleLog(string message, DateTime time, Color color, LogType logType, bool unityHidden = false)
        {
            Type = logType;
            Time = time;
            Message = message ?? string.Empty;
            LogColor = color;
            UnityHidden = unityHidden;
        }

        public static GameConsoleLog CreateNow(string message, string color, LogType logType, bool unityHidden = false) =>
            new GameConsoleLog(message, DateTime.Now, color, logType, unityHidden);

        public static GameConsoleLog CreateNow(string message, Color color, LogType logType, bool unityHidden = false) =>
            new GameConsoleLog(message, DateTime.Now, color, logType, unityHidden);

        public string ToText()
        {
            string log = Message.Replace('\r','\n');
            switch (Type)
            {
                case LogType.User:
                    log = $">{Regex.Replace(log, "<.*?>", string.Empty)}";
                    break;
                case LogType.Game:
                    log = $" {log}";
                    break;
                case LogType.Clear:
                    log = " !clear";
                    break;
                case LogType.Internal:
                    log = " !internal";
                    break;
                default:
                    log = $"?{log}";
                    break;
            }
            string colorHash = ColorUtility.ToHtmlStringRGB(string.IsNullOrWhiteSpace(ColorName) ? LogColor : GameConsoleController.GetColor(ColorName));
            log = $"<color=#{colorHash}>[{Time:HH:mm:ss}]{log}</color>";
            return log;
        }
    }
}