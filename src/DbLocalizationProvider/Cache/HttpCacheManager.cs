using System.Web;

namespace DbLocalizationProvider.Cache
{
    public class HttpCacheManager : ICacheManager
    {
        public void Insert(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value);
        }

        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;
    }
}
