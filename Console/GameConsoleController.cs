﻿using System.Collections.Generic;
using UnityEngine;
using qASIC.Console.Commands;
using qASIC.Console.Logic;
using qASIC.Console.Tools;

namespace qASIC.Console
{
    public static class GameConsoleController
    {
        public static List<GameConsoleLog> Logs = new List<GameConsoleLog>();

        #region Log
        /// <param name="color">color name from the color settings</param>
        public static void Log(string text, string color) => Log(new GameConsoleLog(text, System.DateTime.Now, color, GameConsoleLog.LogType.game));
        /// <param name="color">color name from the color settings</param>
        public static void Log(string text, string color, GameConsoleLog.LogType type) => Log(new GameConsoleLog(text, System.DateTime.Now, color, type));
        public static void Log(string text, Color color) => Log(new GameConsoleLog(text, System.DateTime.Now, color, GameConsoleLog.LogType.game));
        public static void Log(string text, Color color, GameConsoleLog.LogType type) => Log(new GameConsoleLog(text, System.DateTime.Now, color, type));

        public static void Log(GameConsoleLog log)
        {
            if (Logs.Count == 0 && TryGettingConfig(out GameConsoleConfig config) && config.ShowThankYouMessage)
                Logs.Add(new GameConsoleLog("Thank you for using qASIC console", System.DateTime.Now, "qasic", GameConsoleLog.LogType.game));
            Logs.Add(log);
            if (_config != null && _config.LogToUnity && log.Type != GameConsoleLog.LogType.user) Debug.Log($"qASIC game console: {log.Message}");
        }

        /// <summary>Get color from color settings</summary>
        public static Color GetColor(string colorName)
        {
            if (_config == null || _config.ColorTheme == null) return new Color(1f, 1f, 1f);
            colorName = colorName.ToLower();

            if (colorName == "default") return _config.ColorTheme.defaultColor;
            if (colorName == "error") return _config.ColorTheme.errorColor;
            if (colorName == "qasic") return _config.ColorTheme.qASICColor;

            for (int i = 0; i < _config.ColorTheme.colors.Length; i++)
                if (_config.ColorTheme.colors[i].colorName.ToLower() == colorName)
                    return _config.ColorTheme.colors[i].color;
            return _config.ColorTheme.defaultColor;
        }
        #endregion

        #region Config
        private static GameConsoleConfig _config;
        public static GameConsoleConfig GetConfig() { return _config; }
        public static bool TryGettingConfig(out GameConsoleConfig _config) { _config = GameConsoleController._config; return _config != null; }
        public static void AssignConfig(GameConsoleConfig newConfig)
        {
            if (_config == newConfig) return;
            _config = newConfig;
            if (_config.LogConfigAssigment) Log("Assigned new config", "sync", GameConsoleLog.LogType.game);
        }
        #endregion

        #region Logic
        /// <returns>Returns arguments</returns>
        private static List<string> SortCommand(string cmd)
        {
            List<string> args = new List<string>();

            bool isAdvanced = false;
            string currentString = "";

            for (int i = 0; i < cmd.Length; i++)
            {
                if (isAdvanced)
                {
                    if (cmd[i] == '"' && (cmd.Length > i + 1 && cmd[i + 1] == ' ' || cmd.Length > i))
                    {
                        isAdvanced = false;
                        continue;
                    }
                    currentString += cmd[i];
                    continue;
                }
                if (cmd[i] == ' ')
                {
                    args.Add(currentString);
                    currentString = "";
                    continue;
                }
                if (cmd[i] == '"' && (i != 0 && cmd[i - 1] == ' ' || i == 0) && currentString == "")
                {
                    isAdvanced = true;
                    continue;
                }
                currentString += cmd[i];
            }
            args.Add(currentString);
            return args;
        }
                
        /// <summary>Convert logs to text</summary>
        public static string LogsToString(int logLimit)
        {
            string log = "";
            for (int i = 0; i < logLimit; i++)
            {
                if (i >= Logs.Count) break;
                int index = Mathf.Clamp(Logs.Count - logLimit, 0, int.MaxValue) + i;
                if (Logs[index].Type == GameConsoleLog.LogType.clear) log = "";
                else if (i != 0) log += $"\n{Logs[index].ToText()}";
                else log += Logs[index].ToText();
            }
            return log;
        }
        #endregion

        public static void RunCommand(string cmd)
        {
            List<string> args = SortCommand(cmd);
            if (args.Count == 0) return;

            if (GameConsoleCommandList.TryGettingCommandByName(args[0], out GameConsoleCommand command)) command.Run(args);
            else Log("Command not found!", "error", GameConsoleLog.LogType.game);
        }
    }
}