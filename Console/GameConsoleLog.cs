using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace qASIC.Console.Logic
{
    public class GameConsoleLog
    {
        public enum LogType { user, game, clear };
        public LogType Type { get; }
        public DateTime Time { get; }
        public string Message { get; }
        public Color LogColor { get; }
        public string colorName { get; }

        public GameConsoleLog(string message, DateTime time, string color, LogType logType)
        {
            Type = logType;
            Time = time;
            Message = message;
            LogColor = new Color();
            colorName = color;
        }

        public GameConsoleLog(string message, DateTime time, Color color, LogType logType)
        {
            Type = logType;
            Time = time;
            Message = message;
            LogColor = color;
            colorName = string.Empty;
        }

        public string ToText()
        {
            string log = Message.Replace('\r','\n');
            switch (Type)
            {
                case LogType.user:
                    log = $">{Regex.Replace(log, "<.*?>", string.Empty)}";
                    break;
                case LogType.game:
                    log = $" {log}";
                    break;
                case LogType.clear:
                    log = " !clear";
                    break;
                default:
                    log = $"?{log}";
                    break;
            }
            string colorHash = ColorUtility.ToHtmlStringRGB(colorName == null ? LogColor : GameConsoleController.GetColor(colorName));
            log = $"<color=#{colorHash}>[{Time:HH:mm:ss}]{log}</color>";
            return log;
        }
    }
}