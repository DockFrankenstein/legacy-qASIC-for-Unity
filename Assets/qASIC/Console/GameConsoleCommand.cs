using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public abstract class GameConsoleCommand
    {
        public virtual bool Active { get; } = true;
        public abstract string CommandName { get; }
        public abstract string Description { get; }
        public virtual string Help { get; }
        public virtual string[] Aliases { get; }

        public abstract void Run(List<string> args);

        public bool CheckForArgumentCount(List<string>args, int min, int max)
        {
            if (args.Count - 1 < min)
            {
                LogError("User input - not enough arguments!");
                return false;
            }
            if (args.Count - 1 > max)
            {
                LogError("User input - index is out of range!");
                return false;
            }
            return true;
        }

        public bool CheckForArgumentCount(List<string> args, int amount) =>
            CheckForArgumentCount(args, amount, amount);

        public bool CheckForArgumentCountMin(List<string> args, int min)
        {
            if (args.Count - 1 < min)
            {
                LogError("User input - not enough arguments!");
                return false;
            }
            return true;
        }

        public void Log(string text, string color) =>
            GameConsoleController.Log(text, color, Logic.GameConsoleLog.LogType.Game);

        public void Log(string text, Color color) =>
            GameConsoleController.Log(text, color, Logic.GameConsoleLog.LogType.Game);

        public void LogError(string text) =>
            GameConsoleController.Log(text, "error", Logic.GameConsoleLog.LogType.Game);

        public void NoOptionException(string option) =>
            GameConsoleController.Log($"Option <b>{option}</b> does not exist!", "error", Logic.GameConsoleLog.LogType.Game);

        public void ParseException(string text, string type) =>
            GameConsoleController.Log($"Couldn't parse <b>{text}</b> to {type}!", "error", Logic.GameConsoleLog.LogType.Game);
    }
}