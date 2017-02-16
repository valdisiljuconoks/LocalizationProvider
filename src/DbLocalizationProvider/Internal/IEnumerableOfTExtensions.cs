using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Internal
{
    public static class IEnumerableOfTExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (var element in source)
            {
                if(seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        // source: http://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order/3670089#3670089
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer)
        {
            var cnt = new Dictionary<T, int>(comparer);
            foreach (var s in list1)
            {
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else
                    cnt.Add(s, 1);
            }

            foreach (var s in list2)
            {
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else
                    return false;
            }

            return cnt.Values.All(c => c == 0);
        }
    }
}
