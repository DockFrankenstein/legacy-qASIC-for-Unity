using System.Collections.Generic;
using qASIC.Backend;
using qASIC.InputManagment;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputCommand : GameConsoleCommand
    {
        public override string commandName { get => "input"; }
        public override string description { get => "change, print input"; }
        public override string help { get => "Use change <input name>; print"; }

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
                        if (CheckForArgumentCount(args, 1) && GameConsoleController.TryGettingConfig(out Tools.GameConsoleConfig config) && config.showInputMessages)
                            Print();
                        break;
                    default:
                        NoOptionException(args[1]);
                        break;
                }
        }

        public static void Print()
        {
            string log = "<b>InputKeys.asset</b> has been loaded:";
            List<InputKeyValue> keys = InputManager.GetKeys().values;
            for (int i = 0; i < keys.Count; i++)
                log += "\n" + keys[i].keyName + ": " + keys[i].key;
            GameConsoleController.Log(log, "Input");
        }
    }
}