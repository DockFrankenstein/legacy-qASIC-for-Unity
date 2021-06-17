using UnityEngine;

namespace qASIC.Console.Tools
{
    [CreateAssetMenu(fileName = "NewConsoleConfig", menuName = "qASIC/Console/Console Config")]
    public class GameConsoleConfig : ScriptableObject
    {
        public GameConsoleTheme ColorTheme;
        public bool ShowThankYouMessage = true;
        public bool LogConfigAssigment = true;
        public bool LogScene = true;

        [Header("Unity")]
        public bool LogToUnity = false;
        [Space]
        public bool LogUnityErrorsToConsole = true;
        public bool LogUnityAssertsToConsole = true;
        public bool LogUnityExceptionsToConsole = true;
        public bool LogUnityWarningsToConsole = true;
        public bool LogUnityMessagesToConsole = true;

        [Header("Help")]
        public bool ShowDetailedHelp = true;
        public bool UsePageCommandLimit = true;
        [Min(1)]
        public int PageCommandLimit = 5;
    }
}