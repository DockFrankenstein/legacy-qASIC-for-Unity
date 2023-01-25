using System;
using UnityEngine;

namespace qASIC.Serialization
{
    public class JSONSerializationProvider : SerializationProvider
    {
        public override string SerializerName => "JSON";
        public override string SerializerType => "json";

        public override string SerializeObject(object obj, Type type) =>
            JsonUtility.ToJson(obj);

        public override object DeserializeObject(string json, Type type) =>
            JsonUtility.FromJson(json, type);
    }
}