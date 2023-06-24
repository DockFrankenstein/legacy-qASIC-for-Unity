using System;
using System.Collections.Generic;
using System.Linq;

namespace qASIC
{
    public static class IEnumerableExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
        {
            List<TSource> list = source.ToList();
            var targets = list.Where(x => selector.Invoke(x));

            if (targets.Count() != 1)
                return -1;

            return list.IndexOf(targets.First());
        }

        public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source) where TSource : struct
        {
            if (source.Count() == 0)
                return null;

            return source.FirstOrDefault();
        }

        public static TSource? SingleOrNull<TSource>(this IEnumerable<TSource> source) where TSource : struct
        {
            if (source.Count() != 1)
                return null;

            return source.FirstOrDefault();
        }

        public static IEnumerable<TResult> SelectOfType<TSource, TResult>(this IEnumerable<TSource> source) where TResult : TSource =>
            source
            .Where(x => x is TResult)
            .Select(x => (TResult)x);

        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
                action.Invoke(item);

            return source;
        }
    }
}