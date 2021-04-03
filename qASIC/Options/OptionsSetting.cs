using System;

namespace qASIC.Options
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OptionsSetting : Attribute
    {
        public string Name;
        public Type ValueType;

        public OptionsSetting(string name, Type type)
        {
            Name = name;
            ValueType = type;
        }
    }
}