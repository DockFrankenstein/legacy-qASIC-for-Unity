using UnityEngine;

namespace qASIC.Console.Tools
{
    [CreateAssetMenu(fileName = "NewConsoleConfig", menuName = "qASIC/Console/Console Config")]
    public class GameConsoleConfig : ScriptableObject
    {
        public GameConsoleTheme ColorTheme;
        public bool ShowThankYouMessage = true;
        public bool LogToUnity = false;
        public bool LogConfigAssigment = true;
        public bool ShowInputMessages = true;
        public bool LogScene = true;
    }
}