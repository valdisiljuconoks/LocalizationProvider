// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;

namespace DbLocalizationProvider;

/// <summary>
/// Some of the List of T extensions
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Deconstructs list into head (1 element) and tail (sequence of the rest of the elements)
    /// </summary>
    /// <typeparam name="T">List type parameter</typeparam>
    /// <param name="list">Target list</param>
    /// <param name="head">Head of the list</param>
    /// <param name="tail">Tail of the list</param>
    public static void Deconstruct<T>(this List<T> list, out T head, out List<T> tail)
    {
        head = list.FirstOrDefault();
        tail = new List<T>(list.Skip(1));
    }
}
