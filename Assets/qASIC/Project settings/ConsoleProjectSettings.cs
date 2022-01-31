using UnityEngine;
using qASIC.Console.Tools;

namespace qASIC.ProjectSettings
{
    [System.Serializable]
    [ExcludeFromPreset]
    //[CreateAssetMenu(fileName = "NewConsoleProjectSettings", menuName = "qASIC/Project Setting Files/Console")]
    public class ConsoleProjectSettings : ProjectSettingsBase
    {
        private static ConsoleProjectSettings _instance;
        public static ConsoleProjectSettings Instance => CheckInstance("Console", _instance);

        public GameConsoleConfig config;
    }
}