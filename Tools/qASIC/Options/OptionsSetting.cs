using System;

namespace qASIC.Options
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OptionsSetting : Attribute
    {
        public string name;
        public Type type;

        public OptionsSetting(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
    }
}