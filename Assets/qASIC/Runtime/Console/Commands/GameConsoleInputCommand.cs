using UnityEngine;
using System.Collections.Generic;
using qASIC.Input;
using System;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().changeInputCommand; }
        public override string CommandName { get; } = "changeinput";
        public override string Description { get; } = "changes input preference";
        public override string Help { get; } = "Use changeinput <group> <item> <index> <key>; changeinput <group> <item> <key>; changeinput <item> <key>";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 2, 4)) return;

            if (!InputManager.MapLoaded)
            {
                LogError("Input Map has not been assigned!");
                return;
            }

            string key = args[args.Count - 1];

            int index = 0;

            switch (args.Count)
            {
                case 3:
                    InputManager.ChangeInput(args[1], 0, key);
                    return;
                case 4:
                    //User isn't parsing key index
                    if (!int.TryParse(args[2], out int keyIndex)) break;

                    InputManager.ChangeInput(args[1], keyIndex, key);
                    return;
                case 5:
                    if (int.TryParse(args[3], out index)) break;
                    ParseException(args[3], "int");
                    return;
            }

            InputManager.ChangeInput(args[1], args[2], index, key);
        }
    }
}