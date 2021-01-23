using System.Collections.Generic;
using qASIC.Backend;
using qASIC.InputManagment;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleInputCommand : GameConsoleCommand
    {
        public string commandName { get; set; }
        public string description { get; set; }

        public void Run(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 1, 2))
            {
                switch (args[1])
                {
                    case "change":
                        if (GameConsoleController.CheckForArgumentCount(args, 2))
                            qASICObjectsCreation.CreateInputWindow(args[2]);
                        break;
                    case "print":
                        if (GameConsoleController.CheckForArgumentCount(args, 1))
                            Print();
                        break;
                    default:
                        NoOptionEsception(args[1]);
                        break;
                }
            }
        }

        private void NoOptionEsception(string option) =>
            GameConsoleController.Log("Option <b>" + option + "</b> does not exist! Use <b>change</b>; <b>print</b>", "Error1");

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