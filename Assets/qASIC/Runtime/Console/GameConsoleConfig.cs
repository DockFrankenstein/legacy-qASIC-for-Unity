using UnityEngine;
using qASIC.Tools;

namespace qASIC.Console.Internal
{
    [CreateAssetMenu(fileName = "NewConsoleConfig", menuName = "qASIC/Console/Console Config")]
    public class GameConsoleConfig : ScriptableObject
    {
        public GameConsoleTheme colorTheme;
        public bool showThankYouMessage = true;
        public bool logConfigAssigment = true;
        public bool logScene = true;

        //Preferences
        public TextTreeStyle textTreeStyle = new TextTreeStyle(TextTreeStyle.Preset.basic);

        //Built in commands
        public bool clearCommand = true;
        public bool echoCommand = true;
        public bool exitCommand = true;
        public bool helpCommand = true;
        public bool changeInputCommand = true;
        public bool inputListCommand = true;
        public bool changeSettingCommand = true;
        public bool settingListCommand = true;
        public bool audioCommand = true;
        public bool sceneCommand = true;
        public bool sceneListCommand = true;
        public bool versionCommand = true;
        public bool specsCommand = true;
        public bool debugDisplayerCommand = true;
        public bool clearDebugDisplayerCommand = true;
        public bool timeScaleCommand = true;
        public bool fovCommand = true;

        //Unity
        public bool logToUnity = false;
        [Space]
        public bool logUnityErrorsToConsole = true;
        public bool logUnityAssertsToConsole = true;
        public bool logUnityExceptionsToConsole = true;
        public bool logUnityWarningsToConsole = false;
        public bool logUnityMessagesToConsole = false;

        //Version command
        public bool displayGameVersion = true;
        public bool displayUnityVerion = true;
        public bool displayQasicVersion = true;

        //Specs command
        public bool displayCpu = true;
        public bool displayCpuThreads = true;
        public bool displayGpu = true;
        public bool displayMemory = true;
        public bool displaySystem = true;

        //Help command
        public bool showDetailedHelp = true;
        public bool usePageCommandLimit = true;
        [Min(1)]
        public int pageCommandLimit = 5;

        public bool IsReadOnly => readOnly;
        [HideInInspector] [SerializeField] bool readOnly;
    }
}