using System.Collections.Generic;

namespace qASIC
{
    public static class DictionaryExtensions
    {
        public static void SetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            switch (source.ContainsKey(key))
            {
                case true:
                    source[key] = value;
                    break;
                case false:
                    source.Add(key, value);
                    break;
            }
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            if (source.ContainsKey(key))
                return source[key];

            return default;
        }
    }
}