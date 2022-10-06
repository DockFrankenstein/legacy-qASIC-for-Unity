using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qASIC
{
    public static class ListExtensions
    {
        public static bool IndexInRange<TSource>(this List<TSource> source, int index) =>
            index >= 0 && index < source.Count;
    }
}