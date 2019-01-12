// Copyright (c) 2019 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;

namespace DbLocalizationProvider.Cache
{
    internal class BaseCacheManager : ICacheManager
    {
        private ICacheManager _inner;
        internal ConcurrentDictionary<string, object> KnownResourceKeys = new ConcurrentDictionary<string, object>();

        public BaseCacheManager()
        {
        }

        public BaseCacheManager(ICacheManager inner)
        {
            _inner = inner;
        }

        public void Insert(string key, object value)
        {
            VerifyInstance();

            var resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(key);

            _inner.Insert(key, value);
            KnownResourceKeys.TryAdd(resourceKey, null);

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

        internal void StoreKnownKey(string key)
        {
            KnownResourceKeys.TryAdd(key, null);
        }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;

        private void VerifyInstance()
        {
            if(_inner == null)
                throw new InvalidOperationException("Cache implementation is not set. Use `ConfigurationContext.Current.CacheManager` setter.");
        }

        public void SetInnerManager(ICacheManager inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
    }
}
