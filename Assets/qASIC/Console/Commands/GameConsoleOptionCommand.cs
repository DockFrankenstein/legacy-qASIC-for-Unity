using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleOptionCommand : GameConsoleCommand
    {
        public override string commandName { get => "changeoption"; }
        public override string description { get => "changes basic options"; }
        public override string help { get => "Use resolution <x> <y>; fullscreen <state>; vsync <state>; framelock <state>"; }

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 2, 3)) return;
            switch (args.Count)
            {
                case 3:
                    switch (args[1])
                    {
                        case "fullscreen":
                            FullScreen(args);
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
                    break;
                case 4:
                    if (args[1] == "resolution") Resolution(args);
                    else NoOptionException(args[1]);
                    break;
                default:
                    NoOptionException("unknown");
                    break;
            }
        }

        public void FullScreen(List<string> args)
        {
            if (bool.TryParse(args[2], out bool result))
                Options.ChangeFullScreenMode(result);
            else
                ParseException(args[2], "bool");
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
                ParseException(args[2], "int");
        }

        public void Resolution(List<string> args)
        {
            if (int.TryParse(args[2], out int X) && int.TryParse(args[3], out int Y))
                Options.ChangeResolution(X, Y);
            else
                ParseException($"{args[2]}; {args[3]}", "int");
        }
    }
}