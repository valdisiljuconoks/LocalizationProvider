using System;

namespace DbLocalizationProvider.Cache
{
    internal class BaseCacheManager : ICacheManager
    {
        private ICacheManager _inner;

        public void Insert(string key, object value)
        {
            VerifyInstance();
            _inner.Insert(key, value);
            OnInsert?.Invoke(new CacheEventArgs(CacheOperation.Insert, key, CacheKeyHelper.GetResourceKeyFromCacheKey(key)));
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
