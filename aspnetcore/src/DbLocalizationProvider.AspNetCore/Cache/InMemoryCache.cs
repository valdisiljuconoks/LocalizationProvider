// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Concurrent;
using System.Collections.Generic;
using DbLocalizationProvider.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace DbLocalizationProvider.AspNetCore.Cache;

/// <summary>
/// Cache using IMemoryCache as a storage.
/// </summary>
public class InMemoryCache : ICache
{
    private readonly IMemoryCache _memCache;

    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="memCache">Memory cache</param>
    public InMemoryCache(IMemoryCache memCache)
    {
        _memCache = memCache;
    }

    /// <inheritdoc />
    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        _memCache.Set(key, value);
    }

    /// <inheritdoc />
    public object Get(string key)
    {
        return _memCache.Get(key);
    }

    /// <inheritdoc />
    public void Remove(string key)
    {
        _memCache.Remove(key);
    }
}
