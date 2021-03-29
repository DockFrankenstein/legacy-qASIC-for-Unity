using System.Collections.Generic;
using qASIC.Backend;
using qASIC.InputManagement;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "input";
        public override string Description { get; } = "change, print input";
        public override string Help { get; } = "Use input change <input name>; input print";
        public override string[] Aliases { get; } = new string[] { "keys" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 1, 2)) return;
            switch (args[1])
            {
                case "change":
                    if (CheckForArgumentCount(args, 2))
                        qASICObjectsCreation.CreateInputWindow(args[2]);
                    break;
                case "print":
                    if (CheckForArgumentCount(args, 1) && GameConsoleController.TryGettingConfig(out Tools.GameConsoleConfig config) && config.ShowInputMessages)
                        Print();
                    break;
                default:
                    NoOptionException(args[1]);
                    break;
            }
        }

        private static void Print()
        {
            string log = "<b>InputKeys.asset</b> has been loaded:";
            List<string> keys = new List<string>(InputManager.GlobalKeys.Presets.Keys);
            for (int i = 0; i < keys.Count; i++) log += $"\n{keys[i]}: {InputManager.GlobalKeys.Presets[keys[i]]}";
            GameConsoleController.Log(log, "input");
        }
    }
}