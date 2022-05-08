using System.Collections.Generic;
using qASIC.AudioManagement;

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

            if (!AudioManager.Enabled)
            {
                LogError("Audio Manager is disabled!");
                return;
            }

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
                    bool setVolume = false;
                    string s = args[2];

                    if (s.EndsWith("%"))
                    {
                        setVolume = true;
                        s = s.Substring(0, s.Length - 1);
                    }

                    if (!float.TryParse(s, out float newValue))
                    {
                        ParseException(args[2], "float");
                        return;
                    }

                    switch (setVolume)
                    {
                        case true:
                            AudioManager.SetVolume(args[1], newValue / 100f, false);
                            break;
                        case false:
                            AudioManager.SetFloat(args[1], newValue, false);
                            break;
                    }
                    break;
            }
        }
    }
}