using System;

namespace qASIC.Input.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SerializableMapValue : Attribute
    {
        public SerializableMapValue(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}