// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Cache;

internal class BaseCacheManager(ICache inner) : ICacheManager
{
    private readonly ConcurrentDictionary<string, object?> _knownResourceKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, bool> _entries = new(StringComparer.OrdinalIgnoreCase);
    private WeakReference<Dictionary<string, LocalizationResource>>? _allResources;
    internal Func<IServiceProvider, ICache>? _implementationFactory;
    internal ICache _inner = inner;

    internal int KnownKeyCount => _knownResourceKeys.Count;

    internal ICollection<string> KnownKeys => _knownResourceKeys.Keys;

    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        VerifyInstance();

        _inner.Insert(key, value, insertIntoKnownResourceKeys);
        _entries[key] = true;

        string? resourceKey = null;
        if (insertIntoKnownResourceKeys)
        {
            resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(key);
            _knownResourceKeys.TryAdd(resourceKey, null);
        }

        // Insert is the cache-fill path (read from storage, populate cache) - the
        // cached state is consistent with the source of truth, so there is no
        // reason to evict the all-resources dictionary. Mutations invalidate it
        // via Remove instead.

        if (OnInsert is not { } onInsert)
        {
            return;
        }

        resourceKey ??= CacheKeyHelper.GetResourceKeyFromCacheKey(key);
        onInsert(new CacheEventArgs(CacheOperation.Insert, key, resourceKey));
    }

    public object? Get(string key)
    {
        VerifyInstance();
        return _inner.Get(key);
    }

    public void Remove(string key)
    {
        VerifyInstance();

        _inner.Remove(key);
        _entries.TryRemove(key, out _);

        InvalidateAllResourcesDictionary(key);

        if (OnRemove is { } onRemove)
        {
            onRemove(new CacheEventArgs(CacheOperation.Remove, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
        }
    }

    public IEnumerable<string> Keys => _entries.Keys;

    public event CacheEventHandler? OnInsert;
    public event CacheEventHandler? OnRemove;

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

    // The all-resources dictionary is a process-local AdminUI cache. Held by WeakReference so it
    // does not pin the entire resource catalog (every translation for every language) in long-lived
    // heap when the rest of the system is hot-pathing through compact per-key entries.
    internal void InsertAllResourcesDictionary(Dictionary<string, LocalizationResource> allResources)
    {
        _allResources = new WeakReference<Dictionary<string, LocalizationResource>>(allResources);
    }

    internal Dictionary<string, LocalizationResource>? GetAllResourcesDictionary()
    {
        return _allResources is { } weak && weak.TryGetTarget(out var target) ? target : null;
    }

    private void InvalidateAllResourcesDictionary(string key)
    {
        if (string.Equals(key, CacheKeyHelper.AllResourcesCacheKey, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _allResources = null;
    }

    private void VerifyInstance()
    {
        if (_inner == null)
        {
            throw new InvalidOperationException("Cache implementation is not set. Use `ConfigurationContext.CacheManager` setter.");
        }
    }

    public void SetInnerManager(Func<IServiceProvider, ICache>? implementationFactory)
    {
        _implementationFactory = implementationFactory;
    }

    internal void SetInnerManager(ICache implementation)
    {
        _inner = implementation ?? throw new ArgumentNullException(nameof(implementation));
        _implementationFactory = null;
    }
}
