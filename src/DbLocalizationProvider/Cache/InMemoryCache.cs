// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Concurrent;

namespace DbLocalizationProvider.Cache
{
    public class InMemoryCache : ICacheManager
    {
        private static readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
        {
            _cache.TryAdd(key, value);
        }

        public object Get(string key)
        {
            return _cache.GetOrAdd(key, k => null);
        }

        public void Remove(string key)
        {
            _cache.TryRemove(key, out _);
        }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;
    }
}
