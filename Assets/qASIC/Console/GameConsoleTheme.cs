using UnityEngine;

namespace qASIC.Console
{
    [CreateAssetMenu(fileName = "NewConsoleTheme", menuName = "qASIC/Console/Console Theme")]
    public class GameConsoleTheme : ScriptableObject
    {
        [Header("Default")]
        public Color DefaultColor = new Color(1f, 1f, 1f);
        public Color WarningColor = new Color(1f, 1f, 0f);
        public Color ErrorColor = new Color(1f, 0f, 0f);

        [Header("Tools")]
        public Color qASICColor = new Color(0f, 0.7f, 1f);
        public Color SettingsColor = new Color(0f, 0.301f, 1f);
        public Color InputColor = new Color(0.878f, 0.129f, 0f);
        public Color ConsoleColor = new Color(1f, 0f, 1f);
        public Color SceneColor = new Color(0.7019608f, 0f, 1f);
        [Tooltip("The color used by commands that display information")] 
        public Color InfoColor = Color.white;

        [Header("Unity")]
        public Color UnityAssertColor = new Color(1f, 0f, 0f);
        public Color UnityExceptionColor = new Color(1f, 0f, 0f);
        public Color UnityErrorColor = new Color(1f, 0f, 0f);
        public Color UnityWarningColor = new Color(1f, 1f, 0f);
        public Color UnityMessageColor = new Color(1f, 1f, 1f);
        
        [Header("Custom")]
        public GameConsoleColor[] Colors;
    }
}