using UnityEngine;

namespace qASIC.EditorTools
{
    static class RectExtensions
    {
        //Resizing to side
        //Changes rect's size while snapping to the specified side
        public static Rect ResizeToTop(this Rect r, float height)
        {
            r = new Rect(r);
            r.height = height;
            return r;
        }

        public static Rect ResizeToBottom(this Rect r, float height)
        {
            r = new Rect(r);
            r.y += r.height - height;
            r.height = height;
            return r;
        }

        public static Rect ResizeToLeft(this Rect r, float width)
        {
            r = new Rect(r);
            r.width = width;
            return r;
        }

        public static Rect ResizeToRight(this Rect r, float width)
        {
            r = new Rect(r);
            r.x += r.width - width;
            r.width = width;
            return r;
        }

        //Moving
        //The rect gets moved from the specified side and resized to not overflow
        public static Rect MoveTop(this Rect r, float amount)
        {
            r = new Rect(r);
            r.y += amount;
            r.height -= amount;
            return r;
        }

        public static Rect MoveBottom(this Rect r, float amount)
        {
            r = new Rect(r);
            r.y -= amount;
            r.height -= amount;
            return r;
        }

        public static Rect MoveLeft(this Rect r, float amount)
        {
            r = new Rect(r);
            r.x -= amount;
            r.width -= amount;
            return r;
        }

        public static Rect MoveRight(this Rect r, float amount)
        {
            r = new Rect(r);
            r.x += amount;
            r.width -= amount;
            return r;
        }

        //Resizing
        public static Rect BorderRect(this Rect rect, float border) =>
            BorderRect(rect, border, border, border, border);

        public static Rect BorderRect(this Rect rect, float x, float y) =>
            BorderRect(rect, x, x, y, y);

        public static Rect BorderRect(this Rect rect, Vector2 size) =>
            BorderRect(rect, size.x, size.x, size.y, size.y);

        public static Rect BorderRect(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.x += left;
            rect.width -= left + right;
            rect.y += top;
            rect.height -= top + bottom;
            return rect;
        }
    }
}