using System;
using UnityEngine;

namespace qASIC.Files.Serialization
{
    [Serializable]
    public class JSONSerializationProvider : SerializationProvider
    {
        [Tooltip("If true, format the output for readability. If false, format the output for minimum size.")] public bool prettyPrint = true;

        public override string DisplayName => "JSON";
        public override string SerializationType => "json";

        public override string SerializeObject(object obj) =>
            JsonUtility.ToJson(obj, prettyPrint);

        public override object DeserializeObject(string json, Type type) =>
            JsonUtility.FromJson(json, type);
    }
}