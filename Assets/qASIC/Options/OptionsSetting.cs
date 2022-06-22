using System;

namespace qASIC.Options
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OptionsSetting : Attribute
    {
        public string Name { get; }
        public object DefaultValue { get; } = null;
        public string defaultValueMethodName;
        public string enableMethodName;

        [Obsolete]
        public OptionsSetting(string name, Type type)
        {
            Name = name;
        }

        public OptionsSetting(string name, object defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public OptionsSetting(string name)
        {
            Name = name;
        }
    }
}