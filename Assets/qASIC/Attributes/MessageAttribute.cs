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
            switch(icon)
            {           
                case InspectorMessageIconType.notification:
                    Icon = EditorGUIUtility.IconContent("console.infoicon").image;
                    break;
                case InspectorMessageIconType.warning:
                    Icon = EditorGUIUtility.IconContent("console.warnicon").image;
                    break;
                case InspectorMessageIconType.error:
                    Icon = EditorGUIUtility.IconContent("console.erroricon").image;
                    break;
            }
#endif
        }

        public MessageAttribute(string message, Texture icon)
        {
            Message = message;
            Icon = icon;
        }
    }

    public enum InspectorMessageIconType { none, notification, warning, error };
}