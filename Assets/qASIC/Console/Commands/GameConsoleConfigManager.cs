using System.Collections.Generic;
using UnityEngine;
using qASIC.FileManaging;

namespace qASIC.Console.Commands
{
    public class GameConsoleConfigManager : GameConsoleCommand
    {
        public string commandName { get; set; }
        public string description { get; set; }

        public void Run(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 2, -1))
            {
                if (!FileManager.FileExists(args[2]))
                    GameConsoleController.Log("File doesn't exist!", "Error1");
                else
                    switch (args[1])
                    {
                        case "fix":
                            Fix(args);
                            break;
                        case "set":
                            Set(args);
                            break;
                        case "get":
                            Get(args);
                            break;
                        default:
                            NoOptionException(args[1]);
                            break;
                    }
            }
        }

        private void NoOptionException(string option) =>
            GameConsoleController.Log("Option <b>" + option + "</b> does not exist! Use <b>fix</b>l <b>set</b>; <b>get</b>", "Error1");

        private void Fix(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 2))
            {
                FileManager.TryLoadTxtFile(args[2], out string input);
                List<List<string>> output = ConfigController.FixConfig(args[2]);
                GameConsoleController.Log("Fixed config <b>" + args[2] + "</b>:\ninput:\n" +
                    input + "\noutput:\n" + ConfigController.Encode(output), "File");
            }
        }

        private void Get(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 3, 4))
            {
                string groupName = "";
                if (args.Count == 5) groupName = args[4];

                if (ConfigController.TryGettingSetting(args[2], args[3], groupName, out string value))
                    GameConsoleController.Log("Setting <b>" + args[3] + "</b> is equal <b>" + value + "</b>", "File");
                else
                    GameConsoleController.Log("Setting <b>" + args[3] + "</b> does not exist!", "Error1");
            }
        }

        private void Set(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 4, 5))
            {
                string groupName = "";
                if (args.Count == 6) groupName = args[5];

                ConfigController.SaveSetting(args[2], args[3], args[4], groupName);
                GameConsoleController.Log("Value <b>" + args[3] + "</b> has been changed to <b>" + args[4] + "</b> at <b>" + 
                    args[2] + "</b>", "File");
            }
        }
    }
}