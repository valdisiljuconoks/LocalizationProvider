// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider.Internal;

/// <summary>
/// Provides extension methods for <see cref="IEnumerable{T}" />.
/// </summary>
public static class IEnumerableOfTExtensions
{
    /// <summary>
    /// Performs the specified action on each element of the <see cref="IEnumerable{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="action">The action to perform on each element.</param>
    /// <returns>The original source collection.</returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }

        return source;
    }

    /// <summary>
    /// Splits the source collection into smaller collections, each containing the specified number of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <param name="list">The source collection.</param>
    /// <param name="count">The number of elements in each split collection.</param>
    /// <returns>A collection of collections, each containing the specified number of elements.</returns>
    public static IEnumerable<IEnumerable<T>> SplitByCount<T>(this IEnumerable<T> list, int count)
    {
        return list.Select((p, index) => new { p, index })
            .GroupBy(a => a.index / count)
            .Select(grp => grp.Select(g => g.p).ToList());
    }

    /// <summary>
    /// Determines whether two collections contain the same elements, regardless of order, using the specified comparer.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collections.</typeparam>
    /// <param name="list1">The first collection.</param>
    /// <param name="list2">The second collection.</param>
    /// <param name="comparer">The comparer to use for comparing elements.</param>
    /// <returns><c>true</c> if the collections contain the same elements; otherwise, <c>false</c>.</returns>
    public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer)
        where T : notnull
    {
        var cnt = new Dictionary<T, int>(comparer);
        foreach (var s in list1)
        {
            if (!cnt.TryAdd(s, 1))
            {
                cnt[s]++;
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
