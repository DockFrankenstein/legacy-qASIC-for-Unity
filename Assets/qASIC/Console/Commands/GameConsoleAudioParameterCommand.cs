using System.Collections.Generic;
using qASIC.AudioManagment;

namespace qASIC.Console.Commands
{
    public class GameConsoleAudioParameterCommand : GameConsoleCommand
    {
        public override bool Active { get => GameConsoleController.GetConfig().audioCommand; }
        public override string CommandName { get; } = "audioparameter";
        public override string Description { get; } = "changes the specified audio parameters value";
        public override string Help { get; } = "Use audioparameter <parameter>; audioparameter <parameter> <value>";
        public override string[] Aliases { get; } = new string[] { "changeparameter", "changeaudioparameter" };

        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 1, 2)) return;

            switch (args.Count)
            {
                case 2:
                    if (!AudioManager.GetFloat(args[1], out float parameter))
                    {
                        LogError($"Parameter {args[1]} doesn't exist!");
                        return;
                    }

                    Log($"Current value: {parameter}", "audio");
                    break;
                case 3:
                    if (!float.TryParse(args[2], out float newValue))
                    {
                        ParseException(args[2], "float");
                        return;
                    }

                    AudioManager.SetFloat(args[1], newValue, false);
                    break;
            }
        }
    }
}