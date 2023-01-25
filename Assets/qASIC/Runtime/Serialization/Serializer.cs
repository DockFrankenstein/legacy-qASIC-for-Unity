using System;
using UnityEngine;

namespace qASIC.Serialization
{
    [Serializable]
    public abstract class Serializer
    {
        public abstract SerializationProvider Provider { get; }
    }

    public abstract class ObjectSerializer : Serializer
    {
        public override SerializationProvider Provider => provider;

        [SerializeReference] SerializationProvider provider;
    }

    public abstract class GenericSerializer<T> : Serializer
    {
        public override SerializationProvider Provider => provider;

        [SerializeReference] SerializationProvider<T> provider;
    }
}
