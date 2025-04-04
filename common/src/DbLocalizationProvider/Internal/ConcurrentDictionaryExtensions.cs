// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider.Internal;

internal static class ConcurrentDictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue, TArg>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        TArg arg,
        Func<TKey, TArg, TValue> valueFactory) where TKey : notnull
    {
        while (true)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            value = valueFactory(key, arg);

            if (dictionary.TryAdd(key, value))
            {
                return value;
            }
        }
    }
}
