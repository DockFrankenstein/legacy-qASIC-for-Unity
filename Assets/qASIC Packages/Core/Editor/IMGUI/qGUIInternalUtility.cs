#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

using static qASIC.qGUIUtility;

namespace qASIC.EditorTools.Internal
{
    public static class qGUIInternalUtility
    {
        public static Texture2D qASICBackgroundTexture => GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f));
        public static Color qASICColor => qInfo.qASICColor;
        public static Texture2D qASICColorTexture => GenerateColorTexture(qASICColor);

        public static void DrawqASICBanner(string bannerLocation = "qASIC/Sprites/qASIC banner", string docs = "https://docs.qasictools.com")
        {
            Texture2D banner = Resources.Load(bannerLocation) as Texture2D;
            Rect bannerRect = GUILayoutUtility.GetAspectRect((float)banner.width / banner.height).BorderAspectRatioHorizontal(4f);
            GUI.DrawTexture(bannerRect, banner);

            switch (GUILayout.SelectionGrid(-1, new GUIContent[] { new GUIContent("Asset store"), new GUIContent("Docs"), new GUIContent("Support") }, 3))
            {
                case 0:
                    Application.OpenURL("https://qasictools.com/store");
                    break;
                case 1:
                    Application.OpenURL(docs);
                    break;
                case 2:
                    Application.OpenURL("https://qasictools.com/support");
                    break;
            }

            EditorGUILayout.Space();
        }

        public static void BeginGroup() =>
            BeginGroup("");

        public static void BeginGroup(string label)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            if (!string.IsNullOrWhiteSpace(label))
                GUILayout.Label(label, EditorStyles.boldLabel);
        }

        public static void EndGroup(bool space = true)
        {
            if (space)
                EditorGUILayout.Space();

            GUILayout.EndVertical();
        }

        public static void DrawPropertyGroup(SerializedObject serializedObject, string label, string[] properties)
        {
            BeginGroup(label);

            for (int i = 0; i < properties.Length; i++)
                EditorGUILayout.PropertyField(serializedObject.FindProperty(properties[i]));

            EndGroup();
        }

        public class GroupScope : IDisposable
        {
            public GroupScope(bool space = true) : this("", space) { }

            public GroupScope(string label, bool space = true)
            {
                _space = space;
                BeginGroup(label);
            }

            bool _space;

            void IDisposable.Dispose()
            {
                EndGroup(_space);
            }
        }

        public static class Styles
        {
            public static GUIStyle OpenButton => new GUIStyle("Button") { fixedHeight = 24f };
        }
    }
}
#endif