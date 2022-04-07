#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace qASIC.EditorTools
{
    static class GenericMenuExtensions
    {
        /// <summary>Depending on the state, it will add a normal item or a disabled one</summary>
        public static void AddToggableItem(this GenericMenu menu, GUIContent content, bool on, GenericMenu.MenuFunction func, bool enabled)
        {
            switch(enabled)
            {
                case true:
                    menu.AddItem(content, on, func);
                    break;
                default:
                    menu.AddDisabledItem(content, on);
                    break;
            }
        }

        /// <summary>Depending on the state, it will add a normal item or a disabled one</summary>
        public static void AddToggableItem(this GenericMenu menu, string text, bool on, GenericMenu.MenuFunction func, bool enabled) =>
            AddToggableItem(menu, new GUIContent(text), on, func, enabled);

        public static void AddItem(this GenericMenu menu, string text, bool on, GenericMenu.MenuFunction func) =>
            menu.AddItem(new GUIContent(text), on, func);

        public static void AddItem(this GenericMenu menu, string text, bool on, GenericMenu.MenuFunction2 func, object userData) =>
            menu.AddItem(new GUIContent(text), on, func, userData);

        public static void AddDisabledItem(this GenericMenu menu, string text) =>
            menu.AddDisabledItem(new GUIContent(text));

        public static void AddDisabledItem(this GenericMenu menu, string text, bool on) =>
            menu.AddDisabledItem(new GUIContent(text), on);
    }
}

#endif