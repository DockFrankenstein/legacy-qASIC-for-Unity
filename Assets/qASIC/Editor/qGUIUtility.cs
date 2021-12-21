using UnityEngine;

namespace qASIC.UnityEditor
{
    public static class qGUIUtility
    {
        public static Texture2D GenerateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static void DrawMessageBox(string message, InspectorMessageIconType icon) =>
            DrawMessageBox(message, MessageAttribute.ConvertIconTypeToTexture(icon));

        public static void DrawMessageBox(string message, Texture icon = null)
        {
            GUIStyle style = new GUIStyle("Label")
            {
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
            };

            icon = Tools.TextureUtility.Resize((Texture2D)icon, 40, 40, FilterMode.Bilinear);

            GUILayout.BeginHorizontal(new GUIStyle() { normal = new GUIStyleState() { background = GenerateColorTexture(new Color(0f, 0f, 0f, 0.2f)) }, padding = new RectOffset(4, 4, 4, 4) });
            GUILayout.Box(icon, GUILayout.Width(40), GUILayout.Height(40));
            GUILayout.Label(message, style);

            GUILayout.EndHorizontal();
        }
    }
}