using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace qASIC.Files.Serialization
{
    [Serializable]
    public class BinarySerializationProvider : SerializationProvider
    {
        public override string DisplayName => "Binary";
        public override string SerializationType => "binary";

        public override string SerializeObject(object obj)
        {
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return Convert.ToBase64String(stream.ToArray());
        }

        public override object DeserializeObject(string txt, Type type)
        {
            var stream = new MemoryStream(Convert.FromBase64String(txt));
            var formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }
    }
}