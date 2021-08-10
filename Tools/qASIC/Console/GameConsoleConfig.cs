using UnityEngine;

namespace qASIC.Console.Tools
{
    [CreateAssetMenu(fileName = "NewConsoleConfig", menuName = "qASIC/Console/Console Config")]
    public class GameConsoleConfig : ScriptableObject
    {
        public GameConsoleTheme colorTheme;
        public bool showThankYouMessage = true;
        public bool logConfigAssigment = true;
        public bool logScene = true;

        [Header("Built in commands")]
        public bool clearCommand = true;
        public bool echoCommand = true;
        public bool exitCommand = true;
        public bool helpCommand = true;
        public bool inputCommand = true;
        public bool optionCommand = true;
        public bool sceneCommand = true;

        [Header("Unity")]
        public bool logToUnity = false;
        [Space]
        public bool logUnityErrorsToConsole = true;
        public bool logUnityAssertsToConsole = true;
        public bool logUnityExceptionsToConsole = true;
        public bool logUnityWarningsToConsole = true;
        public bool logUnityMessagesToConsole = true;

        [Header("Help")]
        public bool showDetailedHelp = true;
        public bool usePageCommandLimit = true;
        [Min(1)]
        public int pageCommandLimit = 5;
    }
}