using System;

namespace DbLocalizationProvider.Cache {
    internal class BaseCacheManager : ICacheManager
    {
        private ICacheManager _inner = new HttpCacheManager();

        public void Insert(string key, object value)
        {
            _inner.Insert(key, value);
            OnInsert?.Invoke(new CacheEventArgs(CacheOperation.Insert, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
        }

        public object Get(string key)
        {
            return _inner.Get(key);
        }

        public void Remove(string key)
        {
            _inner.Remove(key);
            OnRemove?.Invoke(new CacheEventArgs(CacheOperation.Remove, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
        }

        public void SetInnerManager(ICacheManager inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;
    }
}