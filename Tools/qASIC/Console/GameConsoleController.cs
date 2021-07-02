﻿using System.Collections.Generic;
using UnityEngine;
using qASIC.Console.Commands;
using qASIC.Console.Logic;
using qASIC.Console.Tools;
using UnityEngine.Events;

namespace qASIC.Console
{
    public static class GameConsoleController
    {
        public static List<GameConsoleLog> Logs = new List<GameConsoleLog>();
        public static List<string> InvokedCommands = new List<string>();

        public static UnityAction<GameConsoleLog> OnLog;

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
            if (_config != null && _config.LogToUnity && log.Type != GameConsoleLog.LogType.user && !log.UnityHidden) Debug.Log($"qASIC game console: {log.Message}");
            OnLog?.Invoke(log);
        }

        /// <summary>Get color from color settings</summary>
        public static Color GetColor(string colorName)
        {
            if (_config == null || _config.ColorTheme == null) return new Color(1f, 1f, 1f);
            colorName = colorName.ToLower();

            //base colors
            switch(colorName)
            {
                case "default":
                    return _config.ColorTheme.DefaultColor;
                case "warning":
                    return _config.ColorTheme.WarningColor;
                case "error":
                    return _config.ColorTheme.ErrorColor;
                case "qasic":
                    return _config.ColorTheme.qASICColor;
                case "settings":
                    return _config.ColorTheme.SettingsColor;
                case "input":
                    return _config.ColorTheme.InputColor;
                case "scene":
                    return _config.ColorTheme.SceneColor;
                case "unity exception":
                    return _config.ColorTheme.UnityExceptionColor;
                case "unity error":
                    return _config.ColorTheme.UnityErrorColor;
                case "unity assert":
                    return _config.ColorTheme.UnityAssertColor;
                case "unity warning":
                    return _config.ColorTheme.UnityWarningColor;
                case "unity message":
                    return _config.ColorTheme.UnityMessageColor;
                case "console":
                    return _config.ColorTheme.ConsoleColor;
            }

            for (int i = 0; i < _config.ColorTheme.Colors.Length; i++)
                if (_config.ColorTheme.Colors[i].colorName.ToLower() == colorName)
                    return _config.ColorTheme.Colors[i].color;
            return _config.ColorTheme.DefaultColor;
        }
        #endregion

        #region Config
        private static GameConsoleConfig _config;

        public static bool TryGettingConfig(out GameConsoleConfig config) 
        { 
            config = _config;
            return config != null;
        }

        public static GameConsoleConfig GetConfig()
        {
            if(_config == null)
            {
                GameConsoleConfig defaultConfig = Resources.Load<GameConsoleConfig>("Console/DefaultConfig");

                if(defaultConfig == null)
                {
                    Debug.LogError("Internal qASIC exception! Couldn't locate default configuration for Game Console. Package has been modified or corrupted. Please reinstall or update!");
                    return _config;
                }

                Log("Console configuration isn't assigned, default has been loaded", "warning");
                _config = defaultConfig;
            }

            return _config;
        }

        public static void AssignConfig(GameConsoleConfig newConfig)
        {
            if (_config == newConfig) return;
            _config = newConfig;
            if (_config.LogConfigAssigment) Log("Assigned new config", "console");
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
            string log = string.Empty;
            for (int i = 0; i < logLimit; i++)
            {
                if (i >= Logs.Count) break;
                int index = Mathf.Clamp(Logs.Count - logLimit, 0, int.MaxValue) + i;
                if (Logs[index].Type == GameConsoleLog.LogType.clear) log = "";
                else if (log != string.Empty) log += $"\n{Logs[index].ToText()}";
                else log += Logs[index].ToText();
            }
            return log;
        }
        #endregion

        public static void RunCommand(string cmd)
        {
            if(InvokedCommands.Count == 0 || InvokedCommands[InvokedCommands.Count - 1].ToLower() != cmd.ToLower())
                InvokedCommands.Add(cmd);

            List<string> args = SortCommand(cmd);
            if (args.Count == 0) return;

            if (!GameConsoleCommandList.TryGettingCommandByName(args[0].ToLower(), out GameConsoleCommand command))
            {
                Log("Command not found!", "error");
                return;
            }

            command.Run(args);
        }
    }
}