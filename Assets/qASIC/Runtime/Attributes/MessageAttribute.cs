using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace qASIC
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class MessageAttribute : PropertyAttribute
    {
        public string Message { get; private set; }
        public InspectorMessageIconType IconType { get; private set; }

        public MessageAttribute(string message)
        {
            Message = message;
            IconType = InspectorMessageIconType.none;
        }

        public MessageAttribute(string message, InspectorMessageIconType iconType)
        {
            Message = message;
            IconType = iconType;
        }
    }

    public enum InspectorMessageIconType { none, notification, warning, error };
}