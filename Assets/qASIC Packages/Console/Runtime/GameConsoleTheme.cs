using UnityEngine;

namespace qASIC.Console.Internal
{
    [CreateAssetMenu(fileName = "NewConsoleTheme", menuName = "qASIC/Console/Console Theme")]
    public class GameConsoleTheme : ScriptableObject
    {
        //Default
        public Color DefaultColor = new Color(1f, 1f, 1f);
        public Color WarningColor = new Color(1f, 1f, 0f);
        public Color ErrorColor = new Color(1f, 0f, 0f);
        [Tooltip("The color used by commands that display information")] 
        public Color InfoColor = Color.white;

        //Tools
        public Color qASICColor = new Color(0f, 0.7f, 1f);
        public Color SettingsColor = new Color(0f, 0.301f, 1f);
        public Color InputColor = new Color(1f, 0.1882352941176471f, 0.3450980392156863f);
        public Color AudioColor = new Color(0.9137254901960784f, 0f, 0.6156862745098039f);
        public Color ConsoleColor = new Color(0f, 0.518f, 0.663f);
        public Color SceneColor = new Color(0.7019608f, 0f, 1f);
        [InspectorLabel("Initialization color")] public Color InitColor = new Color(0.3f, 0.3f, 0.3f);

        //Unity
        public Color UnityAssertColor = new Color(1f, 0f, 0f);
        public Color UnityExceptionColor = new Color(1f, 0f, 0f);
        public Color UnityErrorColor = new Color(1f, 0f, 0f);
        public Color UnityWarningColor = new Color(1f, 1f, 0f);
        public Color UnityMessageColor = new Color(1f, 1f, 1f);
        
        //Custom
        public GameConsoleColor[] Colors = new GameConsoleColor[0];

        public bool IsReadOnly => isReadOnly;
#if qASIC_DEV
        [HideInInspector]
#endif
        [SerializeField] bool isReadOnly;
    }
}