﻿using UnityEngine;
using System.Collections.Generic;
using qASIC.InputManagement;
using System;

namespace qASIC.Console.Commands
{
    public class GameConsoleChangeInputCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().inputCommand; }
        public override string CommandName { get; } = "changeinput";
        public override string Description { get; } = "changes input preference";
        public override string Help { get; } = "Use changeinput <group> <action> <index> <key>; changeinput <group> <action> <key>; changeinput <action> <key>";

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 2, 4)) return;

            if (!Enum.TryParse(args[args.Count - 1], out KeyCode key))
            {
                ParseException(args[args.Count - 1], nameof(KeyCode));
                return;
            }

            int index = 0;

            switch (args.Count)
            {
                case 3:
                    InputManager.ChangeInput(args[1], 0, key);
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