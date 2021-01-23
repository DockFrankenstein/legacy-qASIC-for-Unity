using UnityEngine;

namespace qASIC.Console.Tools
{
    [CreateAssetMenu(fileName = "NewConsoleConfig", menuName = "Console Config")]
    public class GameConsoleConfig : ScriptableObject
    {
        public GameConsoleTheme colorTheme;
        public bool showThankYouMessage = true;
        public bool logToUnity = false;
        public bool logConfigAssigment = true;
        public bool showInputMessages = true;
    }
}