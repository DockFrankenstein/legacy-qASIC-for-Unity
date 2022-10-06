using UnityEngine;

namespace qASIC
{
    public static class GUIStyleExtensions
    {
        public static GUIStyle WithBackgroundColor(this GUIStyle style, Color color) =>
            WithBackground(style, qGUIUtility.GenerateColorTexture(color));

        public static GUIStyle WithBackground(this GUIStyle style, Texture2D background)
        {
            style.normal.background = background;
            return style;
        }
    }
}