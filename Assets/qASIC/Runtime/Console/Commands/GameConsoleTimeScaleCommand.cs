using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleTimeScaleCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().timeScaleCommand; }
        public override string CommandName { get; } = "timescale";
        public override string Description { get; } = "changes the time scale";
        public override string Help { get; } = "Use timescale; timescale <value>";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;
            
            switch(args.Count)
            {
                case 2:
                    if(!float.TryParse(args[1], out float newValue))
                    {
                        ParseException(args[1], "float");
                        return;
                    }

                    Time.timeScale = newValue;
                    Log($"Time scale has been changed to {newValue}", "info");
                    break;
                default:
                    Log($"Current timescale: {Time.timeScale}", "info");
                    break;
            }
        }
    }
}