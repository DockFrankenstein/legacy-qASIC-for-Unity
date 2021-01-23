using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleOptionCommand : GameConsoleCommand
    {
        public string commandName { get; set; }
        public string description { get; set; }

        public void Run(List<string> args)
        {
            if (GameConsoleController.CheckForArgumentCount(args, 2, 3))
            {
                if (args[1] == "resolution" && args.Count == 4) Resolution(args);
                else if (GameConsoleController.CheckForArgumentCount(args, 2))
                        switch (args[1])
                        {
                            case "fullscreen":
                                FullScreen(args);
                                break;
                            case "antialiasing":
                                AntiAlias(args);
                                break;
                            case "vsync":
                                Fps(args, false);
                                break;
                            case "framelock":
                                Fps(args, true);
                                break;
                            default:
                            NoOptionException(args[1]);
                                break;
                        }
            }
        }

        private void NoOptionException(string option) => 
            GameConsoleController.Log("Option <b>" + option + "</b> does not exist! Use <b>Fullscreen</b>; <b>Antialiasing</b>; <b>vsync</b>; <b>framelock</b>", "Error1");

        public void FullScreen(List<string> args)
        {
            if (bool.TryParse(args[2], out bool result))
                Options.ChangeFullScreenMode(result);
            else
                GameConsoleController.Log("Couldn't parse arguments!", "Error2");
        }

        public void AntiAlias(List<string> args)
        {
            if (int.TryParse(args[2], out int result))
                Options.ChangeAntiAliasing(result);
            else
                GameConsoleController.Log("Couldn't parse arguments!", "Error2");
        }

        public void Fps(List<string> args, bool lockFrames)
        {
            if (int.TryParse(args[2], out int result))
            {
                if(lockFrames)
                    Options.ChangeFrameOptions(0, result);
                else
                    Options.ChangeFrameOptions(result, -1);
            }
            else
                GameConsoleController.Log("Couldn't parse arguments!", "Error2");
        }

        public void Resolution(List<string> args)
        {
            if (int.TryParse(args[2], out int X) && int.TryParse(args[3], out int Y))
                Options.ChangeResolution(X, Y);
            else
                GameConsoleController.Log("Couldn't parse arguments!", "Error2");
        }
    }
}