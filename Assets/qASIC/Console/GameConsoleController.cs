using System.Collections.Generic;
using UnityEngine;
using qASIC.Console.Commands;

namespace qASIC.Console
{
    public static class GameConsoleController
    {
        public static bool useUnityConsole = true;
        private static string configPath = "/ProjectSettings/qASIC/ConsoleConfiguration.asset";

        public static void Log(string text, Color color)
        {
            string colorText = "<color=#" + ColorUtility.ToHtmlStringRGB(color).ToString() + ">";;

            logs += "\n" + colorText + "[" + System.DateTime.Now.ToString("T") + "]" + text + "</color>";
            if (useUnityConsole) Debug.Log("qASIC game console: " + text);
        }

        public static void Log(string text, string color) => Log(text, GetColor(color));

        public static string GetConfigPath()
        { return FileManaging.FileManager.TrimPathEnd(Application.dataPath, 1) + configPath; }

        public static string logs = "";

        private static Dictionary<string, Color> colors = new Dictionary<string, Color>()
        {
            ["Default"] = new Color(1f, 1f, 1f),
            ["qASIC"] = new Color(0f, 0.7f, 1f),
            ["Error"] = new Color(1f, 0f, 0f),
            ["Error1"] = new Color(0.8f, 0f, 0f),
            ["Error2"] = new Color(0.6f, 0f, 0f),
            ["Error"] = new Color(0.4f, 0f, 0f),
            ["Warning"] = new Color(1f, 0.8f, 0f),
            ["Options"] = new Color(0f, 0.3f, 1f),
            ["Scene"] = new Color(0.7f, 0f, 1f),
            ["File"] = new Color(1f, 1f, 0f),
            ["Reference"] = new Color(0f, 0.7f, 0f),
            ["Audio"] = new Color(1f, 0f, 0.5f),
            ["Input"] = new Color(0.8f, 1f, 0f),
            ["Network"] = new Color(0f, 1f, 1f),
            ["Sync"] = new Color(1f, 0f, 1f),
            ["Sudo"] = new Color(0.5f, 0.8f, 0.1f),
        };

        private static Color GetColor(string colorName)
        {
            if (!colors.ContainsKey(colorName)) return new Color(1f, 1f, 1f);
            return colors[colorName];
        }

        public static void LoadConfig()
        {
            if (FileManaging.ConfigController.TryGettingSetting(GetConfigPath(), "Use unity", "", out string Config_UseUnity))
                useUnityConsole = bool.Parse(Config_UseUnity);
        }

        public static bool CheckForArgumentCount(List<string> args, int min = -1, int max = -1)
        {
            if (args.Count - 1 < min && min != -1)
                Log("User input - not enough arguments!", "Error");
            else if (args.Count - 1 > max && max != -1)
                Log("User input - index is out of range!", "Error");
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

        public static void RunCommand(string _string)
        {
            List<string> args = SortCommand(_string);

            if (args.Count != 0)
            {
                if (GameConsoleCommandList.TryGettingCommandByName(args[0], out GameConsoleCommand command))
                    command.Run(args);
                else
                    Log("Command not found!", "Error");
            }
        }
    }
}