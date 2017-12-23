using System.Collections.Concurrent;

namespace DbLocalizationProvider.Cache
{
    public class InMemoryCache : ICacheManager
    {
        private static ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        public void Insert(string key, object value)
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
