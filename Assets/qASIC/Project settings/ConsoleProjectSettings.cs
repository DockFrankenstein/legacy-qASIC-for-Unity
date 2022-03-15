using UnityEngine;
using qASIC.Console.Internal;

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    public class ConsoleProjectSettings : ProjectSettingsBase
    {
        private static ConsoleProjectSettings _instance;
        public static ConsoleProjectSettings Instance => CheckInstance("Console", _instance);

        public GameConsoleConfig config;

        [Tooltip("If enabled whenever a player starts the game with a certain argument, console will not initialize the command list which will leave it empty.")]
        [InspectorLabel("Disable Command List")]
        public bool startArgsDisableCommandInitialization = true;
    }
}