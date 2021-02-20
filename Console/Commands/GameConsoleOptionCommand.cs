using System.Collections.Generic;
using UnityEngine;
using qASIC.Options;

namespace qASIC.Console.Commands
{
    public class GameConsoleOptionCommand : GameConsoleCommand
    {
        public override string CommandName { get => "changeoption"; }
        public override string Description { get => "changes basic options"; }
        public override string Help { get => "Use <setting name> <value>"; }
        public override string[] Aliases { get => new string[] { "option", "options", "settings" }; }

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 2, 3)) return;
            switch (args.Count)
            {
                case 3:
                    OptionsController.ChangeOption(args[1], args[2]);
                    break;
                case 4:

                    if (int.TryParse(args[2], out int vectorIntResultX) && int.TryParse(args[3], out int vectorIntResultY))
                        OptionsController.ChangeOption(args[1], new Vector2Int(vectorIntResultX, vectorIntResultY));
                    else if (float.TryParse(args[2], out float vectorResultX) && float.TryParse(args[3], out float vectorResultY))
                        OptionsController.ChangeOption(args[1], new Vector2(vectorResultX, vectorResultY));
                    else ParseException($"{args[2]} and {args[3]}", "vector");
                    break;
            }
        }
    }
}