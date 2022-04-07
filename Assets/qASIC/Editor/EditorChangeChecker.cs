#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace qASIC.EditorTools
{
    public static class EditorChangeChecker
    {
        #region ChangeCheck
        static Action _onEndChangeCheck;
        public static Action OnEndChangeCheck { get => _onEndChangeCheck; }

        public static void BeginChangeCheck(Action onEndChangeGroup)
        {
            _onEndChangeCheck = onEndChangeGroup;
            BeginChangeCheck();
        }

        public static void BeginChangeCheck() =>
            EditorGUI.BeginChangeCheck();

        public static bool EndChangeCheck()
        {
            bool value = EditorGUI.EndChangeCheck();
            if (value)
                InvokeChangeCheckAction();
            return value;
        }

        public static void Cleanup()
        {
            _onEndChangeCheck = null;
        }

        public static void EndChangeCheckAndCleanup()
        {
            EndChangeCheck();
            Cleanup();
        }

        public static void InvokeChangeCheckAction() =>
            _onEndChangeCheck.Invoke();
        #endregion

        #region Ignorable Button
        public static bool IgnorableButton(Texture image, params GUILayoutOption[] options) =>
            IgnorableButton(new GUIContent(image), GUI.skin.button, options);

        public static bool IgnorableButton(string text, params GUILayoutOption[] options) =>
            IgnorableButton(new GUIContent(text), GUI.skin.button, options);

        public static bool IgnorableButton(GUIContent content, params GUILayoutOption[] options) =>
            IgnorableButton(content, GUI.skin.button, options);

        public static bool IgnorableButton(Texture image, GUIStyle style, params GUILayoutOption[] options) =>
            IgnorableButton(new GUIContent(image), style, options);

        public static bool IgnorableButton(string text, GUIStyle style, params GUILayoutOption[] options) =>
            IgnorableButton(new GUIContent(text), style, options);

        public static bool IgnorableButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            EndChangeCheck();
            bool value = GUILayout.Button(content, style, options);
            BeginChangeCheck();
            return value;
        }
        #endregion

        //TODO: Add other ignorable items
    }
}

#endif