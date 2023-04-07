using System.Collections.Generic;
using System.Collections;

namespace qASIC.Internal
{
    /// <summary>Contains a list of properties. Normally it's used for displaying debug information</summary>
    public class PropertiesList : IEnumerable<KeyValuePair<string, string>>
    {
        List<KeyValuePair<string, string>> _properties = new List<KeyValuePair<string, string>>();

        public void Add(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, string>(name, value?.ToString() ?? "NULL"));
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public KeyValuePair<string, string> this[int index]
        {
            get { return _properties[index]; }
            set { _properties[index] = value; }
        }
    }
}