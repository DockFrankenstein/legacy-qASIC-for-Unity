using System.Collections.Generic;
using System;

namespace qASIC
{
    public static class ListExtensions
    {
        public static bool IndexInRange<TSource>(this List<TSource> source, int index) =>
            index >= 0 && index < source.Count;

        public static List<TSource> Where<TSource>(this List<TSource> source, Func<int, TSource, bool> func)
        {
            List<TSource> result = new List<TSource>();
            int count = source.Count;
            for (int i = 0; i < count; i++)
            {
                var item = source[i];
                if (func?.Invoke(i, item) == true)
                    result.Add(item);
            }

            return result;
        }
    }
}