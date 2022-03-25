using UnityEngine;

namespace qASIC.Tools
{
    public static class TextureUtility
    {
        public static Texture2D Resize(Texture2D texture, int width, int height, FilterMode filterMode = FilterMode.Bilinear)
        {
            Texture2D newTexture;

            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            rt.filterMode = filterMode;
            texture.filterMode = filterMode;

            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);

            newTexture = new Texture2D(width, height);
            newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            newTexture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return newTexture;
        }
    }
}