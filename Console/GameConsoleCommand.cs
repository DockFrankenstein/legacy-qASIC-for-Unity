using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public abstract class GameConsoleCommand
    {
        public abstract void Run(List<string> args);

        public abstract string commandName { get; }
        public abstract string description { get; }
        public abstract string help { get; }
        public virtual string[] aliases { get; }

        public bool CheckForArgumentCount(List<string>args, int min, int max)
        {
            if (args.Count - 1 < min && min != -1)
                Log("User input - not enough arguments!", "error");
            else if (args.Count - 1 > max && max != -1)
                Log("User input - index is out of range!", "error");
            else return true;
            return false;
        }

        public bool CheckForArgumentCount(List<string> args, int min)
        {
            if (args.Count - 1 < min && min != -1)
                Log("User input - not enough arguments!", "error");
            else return true;
            return false;
        }

        public void Log(string text, string color) =>
            GameConsoleController.Log(text, color, Logic.GameConsoleLog.LogType.game, false);

        public void Log(string text, Color color) =>
            GameConsoleController.Log(text, color, Logic.GameConsoleLog.LogType.game, false);

        public void NoOptionException(string option) =>
            GameConsoleController.Log($"Option <b>{option}</b> does not exist!", "error", Logic.GameConsoleLog.LogType.game, false);

        public void ParseException(string text, string type) =>
            GameConsoleController.Log($"Couldn't parse <b>{text}</b> to {type}!", "error", Logic.GameConsoleLog.LogType.game, false);
    }
}