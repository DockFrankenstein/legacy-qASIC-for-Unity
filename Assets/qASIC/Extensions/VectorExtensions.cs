using UnityEngine;

namespace qASIC
{
    public static class VectorExtensions
    {
        public static string ToStringFormatted(this Vector2? vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector2Int? vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector3? vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector3Int? vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector4? vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector2 vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector2Int vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector3 vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector3Int vector) =>
            VectorText.ToText(vector);

        public static string ToStringFormatted(this Vector4 vector) =>
            VectorText.ToText(vector);
    }
}