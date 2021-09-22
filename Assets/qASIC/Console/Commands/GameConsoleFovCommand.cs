using System.Collections.Generic;
using UnityEngine;

namespace qASIC.Console.Commands
{
    public class GameConsoleFovCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().fovCommand; }
        public override string CommandName { get; } = "fov";
        public override string Description { get; } = "Changes camera field of view";
        public override string Help { get; } = "Use fov; fov <value>";
        public override string[] Aliases { get; } = new string[] { "fieldofview" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0, 1)) return;

            Camera cam = Camera.main;

            switch (args.Count)
            {
                case 2:
                    if (!float.TryParse(args[1], out float newValue))
                    {
                        ParseException(args[1], "float");
                        return;
                    }

                    newValue = Mathf.Clamp(newValue, 1f, 179f);

                    if (!CheckCamera(cam)) return;
                    cam.fieldOfView = newValue;
                    Log($"Field of view has been changed to {newValue}", "info");
                    break;
                default:
                    if (!CheckCamera(cam)) return;
                    Log($"Current field of view: {cam.fieldOfView}", "info");
                    break;
            }
        }

        bool CheckCamera(Camera cam)
        {
            if (cam == null)
            {
                LogError("No camera detected!");
                return false;
            }

            return true;
        }
    }
}