using UnityEditor;
using UnityEngine;

namespace qASIC
{
    public static class RectExtensions
    {
        public static Rect NextLine(this Rect rect)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            return rect;
        }
    }
}