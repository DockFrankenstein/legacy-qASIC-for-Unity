using UnityEngine;

namespace qASIC.Console
{
    [CreateAssetMenu(fileName = "NewConsoleTheme", menuName = "qASIC/Console/Console Theme")]
    public class GameConsoleTheme : ScriptableObject
    {
        public Color DefaultColor = new Color(1f, 1f, 1f);
        public Color ErrorColor = new Color(1f, 0f, 0f);
        public Color qASICColor = new Color(0f, 0.7f, 1f);
        public GameConsoleColor[] Colors;
    }
}