// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider.Cache
{
    internal class BaseCacheManager : ICacheManager
    {
        private ICacheManager _inner;
        internal ConcurrentDictionary<string, object> KnownResourceKeys = new ConcurrentDictionary<string, object>();

        public BaseCacheManager() { }

        public BaseCacheManager(ICacheManager inner)
        {
            _inner = inner;
        }

        public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
        {
            VerifyInstance();

            _inner.Insert(key, value, insertIntoKnownResourceKeys);
            var resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(key);

            if (insertIntoKnownResourceKeys)  KnownResourceKeys.TryAdd(resourceKey, null);

            OnInsert?.Invoke(new CacheEventArgs(CacheOperation.Insert, key, resourceKey));
        }

        public object Get(string key)
        {
            VerifyInstance();
            return _inner.Get(key);
        }

        public void Remove(string key)
        {
            VerifyInstance();
            _inner.Remove(key);

            OnRemove?.Invoke(new CacheEventArgs(CacheOperation.Remove, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
        }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;

        internal void StoreKnownKey(string key)
        {
            KnownResourceKeys.TryAdd(key, null);
        }

        private void VerifyInstance()
        {
            if (_inner == null) throw new InvalidOperationException("Cache implementation is not set. Use `ConfigurationContext.Current.CacheManager` setter.");
        }

        public void SetInnerManager(ICacheManager inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
    }
}
