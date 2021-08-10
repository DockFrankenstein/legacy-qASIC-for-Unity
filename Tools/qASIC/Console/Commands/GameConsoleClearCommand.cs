using System.Collections.Generic;

namespace qASIC.Console.Commands
{
    public class GameConsoleClearCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().clearCommand; }
        public override string CommandName { get; } = "clear";
        public override string Description { get; } = "clears console";
        public override string Help { get; } = "Clears console";
        public override string[] Aliases { get; } = new string[] { "clr", "cls" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            GameConsoleController.Log(string.Empty, "default", Logic.GameConsoleLog.LogType.Clear);
        }
    }
}