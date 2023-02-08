using System;
using UnityEngine;

namespace qASIC.Files.Serialization
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializationObjectTypeAttribute : PropertyAttribute
    {
        public SerializationObjectTypeAttribute(Type type)
        {
            ObjectType = type;
        }

        public Type ObjectType { get; private set; }
    }
}