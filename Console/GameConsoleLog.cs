using UnityEngine;
using System;

namespace qASIC.Console.Logic
{
    public class GameConsoleLog
    {
        public enum LogType { user, game };
        public LogType logType { get; }
        public DateTime time { get; }
        public string message { get; }
        public Color color { get; }
        public bool qASICMessage { get; }

        public GameConsoleLog(string _message, DateTime _time, Color _color, LogType _logType, bool isQASIC)
        {
            logType = _logType;
            time = _time;
            message = _message;
            color = _color;
            qASICMessage = isQASIC;
        }

        public string ToText()
        {
            string log = message;
            switch (logType)
            {
                case LogType.user:
                    log = $">{log}";
                    break;
                case LogType.game:
                    log = $" {log}";
                    break;
                default:
                    log = $"?{log}";
                    if(!qASICMessage) GameConsoleController.Log("Log not recognized", "Error", LogType.game, true);
                    break;
            }
            log = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>[{time:HH:mm:ss}]{log}</color>";
            return log;
        }
    }
}