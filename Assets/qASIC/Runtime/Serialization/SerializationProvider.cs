using System;

namespace qASIC.Serialization
{
    public abstract class SerializationProvider
    {
        public virtual Type ObjectType => typeof(object);
        public abstract string SerializerType { get; }
        public abstract string SerializerName { get; }

        public abstract string SerializeObject(object obj, Type type);
        public abstract object DeserializeObject(string json, Type type);
    }

    public abstract class SerializationProvider<T> : SerializationProvider
    {
        public override Type ObjectType => typeof(T);

        public override string SerializeObject(object obj, Type type) =>
            Serialize((T)obj);
        public override object DeserializeObject(string json, Type type) =>
            Deserialize(json);

        public abstract string Serialize(T obj);
        public abstract T Deserialize(string json);
    }
}