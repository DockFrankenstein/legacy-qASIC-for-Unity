using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public abstract class GameConsoleCommand
    {
        public virtual bool Active { get; } = true;
        public abstract string CommandName { get; }
        public abstract string Description { get; }
        public virtual string Help { get => Description; }
        public virtual string[] Aliases { get; }

        public abstract void Run(List<string> args);

        public virtual bool CheckForArgumentCount(List<string>args, int min, int max)
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

        public virtual bool CheckForArgumentCount(List<string> args, int amount) =>
            CheckForArgumentCount(args, amount, amount);

        public virtual bool CheckForArgumentCountMin(List<string> args, int min)
        {
            if (args.Count - 1 < min)
            {
                LogError("User input - not enough arguments!");
                return false;
            }
            return true;
        }

        protected virtual void Log(string text) =>
            GameConsoleController.Log(text, "default");

        protected virtual void Log(string text, string color) =>
            GameConsoleController.Log(text, color);

        protected virtual void Log(string text, Color color) =>
            GameConsoleController.Log(text, color);

        protected virtual void LogError(string text) =>
            GameConsoleController.Log(text, "error");

        protected virtual void NoOptionException(string option) =>
            GameConsoleController.Log($"Option '{option}' does not exist!", "error");

        protected virtual void ParseException(string text, string type) =>
            GameConsoleController.Log($"Couldn't parse '{text}' to {type}!", "error");
    }
}