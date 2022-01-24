using UnityEngine;
using UnityEditor;

using static qASIC.EditorTools.qGUIUtility;

namespace qASIC.EditorTools.Internal
{
    public static class qGUIInternalUtility
    {
        public static Texture2D qASICBackgroundTexture => GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f));
        public static Color qASICColor => new Color(0f, 0.7019607843137255f, 1f);
        public static Texture2D qASICColorTexture => GenerateColorTexture(qASICColor);

        public static void DrawqASICBanner(string bannerLocation = "qASIC/Sprites/qASIC banner", string docs = "https://docs.qasictools.com")
        {
            Texture2D banner = Resources.Load(bannerLocation) as Texture2D;
            Rect bannerRect = GUILayoutUtility.GetAspectRect((float)banner.width / banner.height).Border(4f);
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

        public static class Styles
        {
            public static GUIStyle OpenButton => new GUIStyle("Button") { fixedHeight = 24f };
        }
    }
}