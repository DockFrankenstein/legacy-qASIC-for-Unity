using UnityEngine;

namespace qASIC
{
    public static class VectorStringConvertion
    {
        #region ToString
        public static string Vector2ToString(Vector2 vector) => $"{vector.x}x{vector.y}";
        public static string Vector2IntToString(Vector2Int vector) => $"{vector.x}x{vector.y}";
        public static string Vector3ToString(Vector3 vector) => $"{vector.x}x{vector.y}";
        public static string Vector3IntToString(Vector3Int vector) => $"{vector.x}x{vector.y}";
        public static string Vector4ToString(Vector4 vector) => $"{vector.x}x{vector.y}";
        #endregion

        #region ToVector
        public static Vector2 StringToVector2(string s)
        {
            float[] values = TryParseValues(GetStringValues(s, 2));
            return new Vector2(values[0], values[1]);
        }

        public static Vector2Int StringToVector2Int(string s)
        {
            int[] values = TryParseValuesInt(GetStringValues(s, 2));
            return new Vector2Int(values[0], values[1]);
        }
        public static Vector3 StringToVector3(string s)
        {
            float[] values = TryParseValues(GetStringValues(s, 3));
            return new Vector3(values[0], values[1], values[2]);
        }

        public static Vector3Int StringToVector3Int(string s)
        {
            int[] values = TryParseValuesInt(GetStringValues(s, 3));
            return new Vector3Int(values[0], values[1], values[2]);
        }

        public static Vector4 StringToVector4(string s)
        {
            float[] values = TryParseValues(GetStringValues(s, 4));
            return new Vector4(values[0], values[1], values[2], values[3]);
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

        private static float[] TryParseValues(string[] values)
        {
            float[] parsedValues = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
                if (!float.TryParse(values[i], out parsedValues[i]))
                    parsedValues[i] = 0;
            return parsedValues;
        }

        private static int[] TryParseValuesInt(string[] values)
        {
            int[] parsedValues = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
                if (!int.TryParse(values[i], out parsedValues[i]))
                    parsedValues[i] = 0;
            return parsedValues;
        }
    }
}