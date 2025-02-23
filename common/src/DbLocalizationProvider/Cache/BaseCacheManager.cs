// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DbLocalizationProvider.Cache;

internal class BaseCacheManager(ICache inner) : ICacheManager
{
    private readonly ConcurrentDictionary<string, object?> _knownResourceKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, bool> _entries = new();
    internal Func<IServiceProvider, ICache>? _implementationFactory;
    internal ICache _inner = inner;

    internal int KnownKeyCount => _knownResourceKeys.Count;

    internal ICollection<string> KnownKeys => _knownResourceKeys.Keys;

    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        VerifyInstance();

        var k = key.ToLower();
        _inner.Insert(k, value, insertIntoKnownResourceKeys);
        _entries.TryRemove(k, out _);
        _entries.TryAdd(k, true);

        var resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(key);

        if (insertIntoKnownResourceKeys)
        {
            _knownResourceKeys.TryAdd(resourceKey, null);
        }

        OnInsert?.Invoke(new CacheEventArgs(CacheOperation.Insert, key, resourceKey));
    }

    public object? Get(string key)
    {
        VerifyInstance();
        return _inner.Get(key.ToLower());
    }

    public void Remove(string key)
    {
        VerifyInstance();

        var k = key.ToLower();
        _inner.Remove(k);
        _entries.TryRemove(k, out _);

        OnRemove?.Invoke(new CacheEventArgs(CacheOperation.Remove, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
    }

    public IEnumerable<string> Keys => _entries.Keys;

    public event CacheEventHandler? OnInsert;
    public event CacheEventHandler? OnRemove;

    internal void SetInnerManager(ICache implementation)
    {
        _inner = implementation ?? throw new ArgumentNullException(nameof(implementation));
        _implementationFactory = null;
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
        _inner.Insert(CacheKeyHelper.BuildKey("KnownKeysSynced"), true, false);
    }

    internal bool AreKnownKeysStored()
    {
        return _inner.Get(CacheKeyHelper.BuildKey("KnownKeysSynced")) as bool? ?? false;
    }

    private void VerifyInstance()
    {
        if (_inner == null)
        {
            throw new InvalidOperationException(
                "Cache implementation is not set. Use `ConfigurationContext.CacheManager` setter.");
        }
    }

    public void SetInnerManager(Func<IServiceProvider, ICache>? implementationFactory)
    {
        _implementationFactory = implementationFactory;
    }
}
