using System.Collections.Generic;
using UnityEngine;
using qASIC.Console.Commands;
using qASIC.Console.Logic;
using qASIC.Console.Tools;

namespace qASIC.Console
{
    public static class GameConsoleController
    {
        public static List<GameConsoleLog> logs = new List<GameConsoleLog>();

        #region Log
        public static void Log(string text, Color color) => Log(text, color, GameConsoleLog.LogType.game, false);
        public static void Log(string text, string color) => Log(text, GetColor(color), GameConsoleLog.LogType.game, false);

        public static void Log(string text, string color, GameConsoleLog.LogType type, bool qASIC) => Log(text, GetColor(color), type, qASIC);
        public static void Log(string text, Color color, GameConsoleLog.LogType type, bool qASIC)
        {
            logs.Add(new GameConsoleLog(text, System.DateTime.Now, color, type, qASIC));
            if (config.logToUnity) Debug.Log("qASIC game console: " + text);
        }

        private static Color GetColor(string colorName)
        {
            if (config == null || config.colorTheme == null) return new Color(1f, 1f, 1f);
            colorName = colorName.ToLower();

            if (colorName == "default") return config.colorTheme.defaultColor;
            if (colorName == "error") return config.colorTheme.errorColor;
            if (colorName == "qasic") return config.colorTheme.qASICColor;

            for (int i = 0; i < config.colorTheme.colors.Length; i++)
                if (config.colorTheme.colors[i].colorName.ToLower() == colorName)
                    return config.colorTheme.colors[i].color;
            return config.colorTheme.defaultColor;
        }
        #endregion

        #region Config
        private static GameConsoleConfig config;
        public static GameConsoleConfig GetConfig() { return config; }
        public static bool TryGettingConfig(out GameConsoleConfig _config) { _config = config; return _config != null; }
        public static void AssignConfig(GameConsoleConfig newConfig)
        {
            if (config == newConfig) return;
            config = newConfig;
            if (config.logConfigAssigment) Log("Assigned new config", "sync", GameConsoleLog.LogType.game, false);
        }
        #endregion

        #region Logic
        public static bool CheckForArgumentCount(List<string> args, int min = -1, int max = -1)
        {
            if (args.Count - 1 < min && min != -1)
                Log("User input - not enough arguments!", "error");
            else if (args.Count - 1 > max && max != -1)
                Log("User input - index is out of range!", "error");
            else return true;

            return false;
        }

        private static List<string> SortCommand(string _string)
        {
            List<string> args = new List<string>();

            char[] chars = _string.ToCharArray();
            bool isComplicated = false;
            string currentString = "";

            for (int i = 0; i < chars.Length; i++)
            {
                if (isComplicated)
                {
                    if (chars[i] == '"' && (chars.Length > i + 1 && chars[i + 1] == ' ' || chars.Length > i))
                        isComplicated = false;
                    else
                        currentString += chars[i];
                }
                else
                {
                    if (chars[i] == ' ')
                    {
                        args.Add(currentString);
                        currentString = "";
                    }
                    else if (chars[i] == '"' && (i != 0 && chars[i - 1] == ' ' || i == 0) && currentString == "")
                        isComplicated = true;
                    else
                        currentString += chars[i];
                }
            }
            args.Add(currentString);

            return args;
        }

        public static string LogToString(int logLimit)
        {
            string log = "";
            for (int i = 0; i < logLimit; i++)
            {
                if (i >= logs.Count) break;
                log += $"\n{logs[Mathf.Clamp(logs.Count - logLimit, 0, int.MaxValue) + i].ToText()}";
            }
            return log;
        }
        #endregion

        public static void RunCommand(string _string)
        {
            List<string> args = SortCommand(_string);

            if (args.Count != 0)
            {
                if (GameConsoleCommandList.TryGettingCommandByName(args[0], out GameConsoleCommand command))
                    command.Run(args);
                else
                    Log("Command not found!", "error");
            }
        }
    }
}