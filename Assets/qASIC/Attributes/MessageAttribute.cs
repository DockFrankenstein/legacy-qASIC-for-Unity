using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace qASIC
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class MessageAttribute : PropertyAttribute
    {
        public string Message { get; private set; }
        public Texture Icon { get; set; }

        public MessageAttribute(string message)
        {
            Message = message;
        }

        public MessageAttribute(string message, InspectorMessageIconType icon)
        {
            Message = message;
#if UNITY_EDITOR
            Icon = ConvertIconTypeToTexture(icon);
#endif
        }

        public static Texture ConvertIconTypeToTexture(InspectorMessageIconType icon)
        {
            switch (icon)
            {
                case InspectorMessageIconType.notification:
                    return EditorGUIUtility.IconContent("console.infoicon").image;
                case InspectorMessageIconType.warning:
                    return EditorGUIUtility.IconContent("console.warnicon").image;
                case InspectorMessageIconType.error:
                    return EditorGUIUtility.IconContent("console.erroricon").image;
                default:
                    return null;
            }
        }

        public MessageAttribute(string message, Texture icon)
        {
            Message = message;
            Icon = icon;
        }
    }

    public enum InspectorMessageIconType { none, notification, warning, error };
}