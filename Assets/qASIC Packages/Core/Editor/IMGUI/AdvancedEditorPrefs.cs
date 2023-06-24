#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace qASIC.EditorTools.Internal
{
    public static class AdvancedEditorPrefs
    {
        public static void SetColor(string key, Color color)
        {
            EditorPrefs.SetFloat($"{key}_r", color.r);
            EditorPrefs.SetFloat($"{key}_g", color.g);
            EditorPrefs.SetFloat($"{key}_b", color.b);
        }

        public static void SetColorWithAlpha(string key, Color color)
        {
            SetColor(key, color);
            EditorPrefs.SetFloat($"{key}_a", color.a);
        }

        public static Color GetColor(string key) =>
            GetColor(key, Color.white);

        public static Color GetColor(string key, Color defaultValue)
        {
            Color color = defaultValue;
            color.r = EditorPrefs.GetFloat($"{key}_r", color.r);
            color.g = EditorPrefs.GetFloat($"{key}_g", color.g);
            color.b = EditorPrefs.GetFloat($"{key}_b", color.b);
            color.a = EditorPrefs.GetFloat($"{key}_a", color.a);
            return color;
        }

        public static void DeleteColorKey(string key)
        {
            EditorPrefs.DeleteKey($"{key}_r");
            EditorPrefs.DeleteKey($"{key}_g");
            EditorPrefs.DeleteKey($"{key}_b");
            EditorPrefs.DeleteKey($"{key}_a");
        }

        public static void SetObject(string key, object o)
        {
            switch (o)
            {
                //Unity
                case int i:
                    EditorPrefs.SetInt(key, i);
                    break;
                case float f:
                    EditorPrefs.SetFloat(key, f);
                    break;
                case bool b:
                    EditorPrefs.SetBool(key, b);
                    break;

                //Custom
                case Color c:
                    SetColorWithAlpha(key, c);
                    break;
                
                //Default
                default:
                    EditorPrefs.SetString(key, o.ToString());
                    break;
            }
        }

        public static T GetObject<T>(string key) =>
            GetObject<T>(key, default);

        public static T GetObject<T>(string key, T defaultValue)
        {
            object value;
            switch (defaultValue)
            {
                //Unity
                case int i:
                    value = EditorPrefs.GetInt(key, i);
                    break;
                case float f:
                    value = EditorPrefs.GetFloat(key, f);
                    break;
                case bool b:
                    value = EditorPrefs.GetBool(key, b);
                    break;

                //Custom
                case Color c:
                    value = GetColor(key, c);
                    break;

                //Default
                default:
                    value = EditorPrefs.GetString(key, defaultValue as string);
                    break;
            }

            return (T)value;
        }
    }
}
#endif