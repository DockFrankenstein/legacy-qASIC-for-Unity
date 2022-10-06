using System.Collections.Generic;
using qASIC.Toggling;
using qASIC.Tools;

namespace qASIC.Console.Commands
{
    public class GameConsoleDebugDisplayerCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().debugDisplayerCommand; }
        public override string CommandName { get; } = "debugdisplayer";
        public override string Description { get; } = "Toggles debug displayer";
        public override string Help { get; } = "Use debugdisplayer; debugdisplayer <value>";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            if (!StaticToggler.states.ContainsKey("debug displayer"))
            {
                qASICObjectCreator.CreateDebugDisplyer();
                Log("Debug displayer has been generated", "info");
                return;
            }

            bool newState;
    
            switch (args.Count)
            {
                case 2:
                    if(!bool.TryParse(args[1], out newState))
                    {
                        ParseException(args[1], "bool");
                        return;
                    }
                    break;
                default:
                    newState = !StaticToggler.states["debug displayer"].state;
                    break;
            }

            StaticToggler.ChangeState("debug displayer", newState);
            Log($"Debug displayer has been {(newState ? "enabled" : "disabled")}", "info");
        }
    }
}