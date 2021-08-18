using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace qASIC.Console.Logic
{
    public class GameConsoleLog
    {
        public enum LogType { User, Game, Clear };
        public LogType Type { get; }
        public DateTime Time { get; }
        public string Message { get; }
        public Color LogColor { get; }
        public string ColorName { get; }
        
        /// <summary>If the log should be hidden in Unity console</summary>
        public bool UnityHidden { get; }

        public GameConsoleLog(string message, DateTime time, string color, LogType logType)
        {
            if (message == null) message = string.Empty;

            Type = logType;
            Time = time;
            Message = message;
            LogColor = new Color();
            ColorName = color;
            UnityHidden = false;
        }

        public GameConsoleLog(string message, DateTime time, Color color, LogType logType)
        {
            if (message == null) message = string.Empty;

            Type = logType;
            Time = time;
            Message = message;
            LogColor = color;
            ColorName = string.Empty;
            UnityHidden = false;
        }

        public GameConsoleLog(string message, DateTime time, string color, LogType logType, bool unityHidden)
        {
            if (message == null) message = string.Empty;

            Type = logType;
            Time = time;
            Message = message;
            LogColor = new Color();
            ColorName = color;
            UnityHidden = unityHidden;
        }

        public GameConsoleLog(string message, DateTime time, Color color, LogType logType, bool unityHidden)
        {
            if (message == null) message = string.Empty;

            Type = logType;
            Time = time;
            Message = message;
            LogColor = color;
            ColorName = string.Empty;
            UnityHidden = unityHidden;
        }

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