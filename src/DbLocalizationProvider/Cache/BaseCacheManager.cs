// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DbLocalizationProvider.Cache;

internal class BaseCacheManager(ICacheManager inner) : ICacheManager
{
    private readonly ConcurrentDictionary<string, object> _knownResourceKeys = new(StringComparer.InvariantCultureIgnoreCase);

    internal int KnownKeyCount => _knownResourceKeys.Count;

    internal ICollection<string> KnownKeys => _knownResourceKeys.Keys;

    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        VerifyInstance();

        inner.Insert(key.ToLower(), value, insertIntoKnownResourceKeys);
        var resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(key);

        if (insertIntoKnownResourceKeys)
        {
            _knownResourceKeys.TryAdd(resourceKey, null);
        }

        OnInsert?.Invoke(new CacheEventArgs(CacheOperation.Insert, key, resourceKey));
    }

    public object Get(string key)
    {
        VerifyInstance();
        return inner.Get(key.ToLower());
    }

    public void Remove(string key)
    {
        VerifyInstance();
        inner.Remove(key.ToLower());

        OnRemove?.Invoke(new CacheEventArgs(CacheOperation.Remove, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
    }

    public event CacheEventHandler? OnInsert;
    public event CacheEventHandler? OnRemove;

    public void SetInnerManager(ICacheManager implementation)
    {
        inner = implementation ?? throw new ArgumentNullException(nameof(implementation));
    }

    internal bool IsKeyKnown(string key)
    {
        return _knownResourceKeys.ContainsKey(key);
    }

    internal void StoreKnownKey(string key)
    {
        _knownResourceKeys.TryAdd(key, null);
    }

    internal void SetKnownKeysStored()
    {
        inner.Insert(CacheKeyHelper.BuildKey("KnownKeysSynched"), true, false);
    }

    internal bool AreKnownKeysStored()
    {
        return inner.Get(CacheKeyHelper.BuildKey("KnownKeysSynched")) as bool? ?? false;
    }

    private void VerifyInstance()
    {
        if (inner == null)
        {
            throw new InvalidOperationException(
                "Cache implementation is not set. Use `ConfigurationContext.CacheManager` setter.");
        }
    }
}
