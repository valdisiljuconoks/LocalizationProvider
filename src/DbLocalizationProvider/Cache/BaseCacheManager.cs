// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider.Cache
{
    internal class BaseCacheManager : ICacheManager
    {
        private ICacheManager _inner;
        private readonly ConcurrentDictionary<string, object> _knownResourceKeys = new ConcurrentDictionary<string, object>();

        public BaseCacheManager() { }

        public BaseCacheManager(ICacheManager inner)
        {
            _inner = inner;
        }

        public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
        {
            VerifyInstance();

            _inner.Insert(key.ToLower(), value, insertIntoKnownResourceKeys);
            var resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(key);

            if (insertIntoKnownResourceKeys) _knownResourceKeys.TryAdd(resourceKey.ToLower(), null);

            OnInsert?.Invoke(new CacheEventArgs(CacheOperation.Insert, key, resourceKey));
        }

        public object Get(string key)
        {
            VerifyInstance();
            return _inner.Get(key.ToLower());
        }

        public void Remove(string key)
        {
            VerifyInstance();
            _inner.Remove(key.ToLower());

            OnRemove?.Invoke(new CacheEventArgs(CacheOperation.Remove, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
        }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;

        public void SetInnerManager(ICacheManager inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        internal bool IsResourceKeyKnown(string key)
        {
            return _knownResourceKeys.ContainsKey(key.ToLower());
        }

        internal void StoreKnownKey(string key)
        {
            _knownResourceKeys.TryAdd(key.ToLower(), null);
        }

        private void VerifyInstance()
        {
            if (_inner == null)
            {
                throw new InvalidOperationException("Cache implementation is not set. Use `ConfigurationContext.Current.CacheManager` setter.");
            }
        }
    }
}
