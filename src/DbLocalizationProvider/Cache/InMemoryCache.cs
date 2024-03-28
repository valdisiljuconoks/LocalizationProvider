// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Concurrent;

namespace DbLocalizationProvider.Cache;

/// <summary>
/// Cache implementation for cases when you have enough memory.
/// </summary>
/// <seealso cref="DbLocalizationProvider.Cache.ICacheManager" />
public class InMemoryCache : ICacheManager
{
    private static readonly ConcurrentDictionary<string, object> _cache = new();

    /// <summary>
    /// You should add given resource to the cache with known cache key.
    /// </summary>
    /// <param name="key">Key identifier of the cached item</param>
    /// <param name="value">Actual value fo the cached item</param>
    /// <param name="insertIntoKnownResourceKeys">This is pretty internal stuff and should be ignored by cache implementers.</param>
    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        _cache.TryRemove(key, out _);
        _cache.TryAdd(key, value);
    }

    /// <summary>
    /// You should implement this method to get cached item back from the underlying storage
    /// </summary>
    /// <param name="key">Key identifier of the cached item</param>
    /// <returns>
    /// Actual value fo the cached item. Take care of casting back to proper type.
    /// </returns>
    public object Get(string key)
    {
        return _cache.GetOrAdd(key, k => null);
    }

    /// <summary>
    /// If you want to remove the cached item from the storage - this is the method to implement then.
    /// </summary>
    /// <param name="key">Key identifier of the cached item</param>
    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }

    /// <summary>
    /// Event raise is taken care by <see cref="BaseCacheManager" />.
    /// </summary>
    public event CacheEventHandler OnInsert;

    /// <summary>
    /// Event raise is taken care by <see cref="BaseCacheManager" />.
    /// </summary>
    public event CacheEventHandler OnRemove;
}
