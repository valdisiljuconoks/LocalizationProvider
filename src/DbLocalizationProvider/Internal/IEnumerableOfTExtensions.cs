// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Internal;

public static class IEnumerableOfTExtensions
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }

        return source;
    }

    public static IEnumerable<IEnumerable<T>> SplitByCount<T>(this IEnumerable<T> list, int count)
    {
        return list.Select((p, index) => new { p, index })
            .GroupBy(a => a.index / count)
            .Select(grp => grp.Select(g => g.p).ToList());
    }

    // source: http://stackoverflow.com/questions/3669970/compare-two-listt-objects-for-equality-ignoring-order/3670089#3670089
    public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer)
    {
        var cnt = new Dictionary<T, int>(comparer);
        foreach (var s in list1)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]++;
            }
            else
            {
                cnt.Add(s, 1);
            }
        }

        foreach (var s in list2)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]--;
            }
            else
            {
                return false;
            }
        }

        return cnt.Values.All(c => c == 0);
    }
}
