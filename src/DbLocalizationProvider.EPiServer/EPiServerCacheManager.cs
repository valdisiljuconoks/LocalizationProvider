using DbLocalizationProvider.Cache;
using EPiServer;

namespace DbLocalizationProvider.EPiServer
{
    internal class EPiServerCacheManager : ICacheManager
    {
        public void Insert(string key, object value)
        {
            CacheManager.Insert(key, value);
        }

        public object Get(string key)
        {
            return CacheManager.Get(key);
        }

        public void Remove(string key)
        {
            CacheManager.Remove(key);
        }
    }
}
