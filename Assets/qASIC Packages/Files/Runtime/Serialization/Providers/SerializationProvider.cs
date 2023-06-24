using System;

namespace qASIC.Files.Serialization
{
    [Serializable]
    public abstract class SerializationProvider
    {
        public virtual Type ObjectType => typeof(object);
        public abstract string SerializationType { get; }
        public abstract string DisplayName { get; }

        /// <summary>Determines if the provider serializes it's data to a file</summary>
        public virtual bool SavesToFile { get; } = true;

        public abstract string SerializeObject(object obj);
        public abstract object DeserializeObject(string txt, Type type);
    }

    [Serializable]
    public abstract class SerializationProvider<T> : SerializationProvider
    {
        public override Type ObjectType => typeof(T);

        public override string SerializeObject(object obj) =>
            Serialize((T)obj);
        public override object DeserializeObject(string txt, Type type) =>
            Deserialize(txt);

        public abstract string Serialize(T obj);
        public abstract T Deserialize(string txt);
    }
}