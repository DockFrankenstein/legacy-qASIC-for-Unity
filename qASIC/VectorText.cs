using UnityEngine;

namespace qASIC
{
    public static class VectorText
    {
        #region ToString
        public static string ToText(Vector2 vector) => $"{vector.x}x{vector.y}";
        public static string ToText(Vector2Int vector) => $"{vector.x}x{vector.y}";
        public static string ToText(Vector3 vector) => $"{vector.x}x{vector.y}x{vector.z}";
        public static string ToText(Vector3Int vector) => $"{vector.x}x{vector.y}x{vector.z}";
        public static string ToText(Vector4 vector) => $"{vector.x}x{vector.y}x{vector.z}x{vector.w}";
        #endregion

        #region TryToVector
        public static bool TryToVector2(string s, out Vector2 vector)
        {
            vector = new Vector2();
            float[] values = ParseValues(GetStringValues(s, 2));
            if (values.Length != 2) return false;
            vector = new Vector2(values[0], values[1]);
            return true;
        }

        public static bool TryToVector2Int(string s, out Vector2Int vector)
        {
            vector = new Vector2Int();
            int[] values = ParseValuesInt(GetStringValues(s, 2));
            if (values.Length != 2) return false;
            vector = new Vector2Int(values[0], values[1]);
            return true;
        }
        public static bool TryToVector3(string s, out Vector3 vector)
        {
            vector = new Vector3();
            float[] values = ParseValues(GetStringValues(s, 3));
            if (values.Length != 3) return false;
            vector = new Vector3(values[0], values[1], values[2]);
            return true;
        }

        public static bool TryToVector3Int(string s, out Vector3Int vector)
        {
            vector = new Vector3Int();
            int[] values = ParseValuesInt(GetStringValues(s, 3));
            if (values.Length != 3) return false;
            vector = new Vector3Int(values[0], values[1], values[2]);
            return true;
        }

        public static bool TryToVector4(string s, out Vector4 vector)
        {
            vector = new Vector4();
            float[] values = ParseValues(GetStringValues(s, 4));
            if (values.Length != 4) return false;
            vector = new Vector4(values[0], values[1], values[2], values[3]);
            return true;
        }
        #endregion

        #region ToVector
        public static Vector2 ToVector2(string s)
        {
            if (!TryToVector2(s, out Vector2 vector)) qDebug.LogError("Couldn't convert string to Vector2");
            return vector;
        }

        public static Vector2Int ToVector2Int(string s)
        {
            if (!TryToVector2Int(s, out Vector2Int vector)) qDebug.LogError("Couldn't convert string to Vector2Int");
            return vector;
        }
        public static Vector3 ToVector3(string s)
        {
            if (!TryToVector3(s, out Vector3 vector)) qDebug.LogError("Couldn't convert string to Vector3");
            return vector;
        }

        public static Vector3Int ToVector3Int(string s)
        {
            if (!TryToVector3Int(s, out Vector3Int vector)) qDebug.LogError("Couldn't convert string to Vector3Int");
            return vector;
        }

        public static Vector4 ToVector4(string s)
        {
            if (!TryToVector4(s, out Vector4 vector)) qDebug.LogError("Couldn't convert string to Vector4");
            return vector;
        }
        #endregion


        private static string[] GetStringValues(string s, int count)
        {
            string[] stringValues = s.Split('x');
            string[] values = new string[count];
            for (int i = 0; i < count; i++)
            {
                if (stringValues.Length > i)
                {
                    values[i] = stringValues[i];
                    continue;
                }
                values[i] = "0";
            }
            return values;
        }

        private static float[] ParseValues(string[] values)
        {
            float[] parsedValues = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
                if (!float.TryParse(values[i], out parsedValues[i]))
                    parsedValues[i] = 0;
            return parsedValues;
        }

        private static int[] ParseValuesInt(string[] values)
        {
            int[] parsedValues = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
                if (!int.TryParse(values[i], out parsedValues[i]))
                    parsedValues[i] = 0;
            return parsedValues;
        }
    }
}