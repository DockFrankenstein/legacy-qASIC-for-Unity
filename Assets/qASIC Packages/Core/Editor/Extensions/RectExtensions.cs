using UnityEditor;
using UnityEngine;

namespace qASIC
{
    public static class RectExtensions
    {
        public static Rect NextLine(this Rect rect, bool addSpacing = true)
        {
            rect.y += rect.height + (addSpacing ? EditorGUIUtility.standardVerticalSpacing : 0f);
            return rect;
        }
    }
}