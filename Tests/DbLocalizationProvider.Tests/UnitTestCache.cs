using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.Tests
{
    public class UnitTestCache : ICacheManager
    {
        public void Insert(string key, object value, bool insertIntoKnownResourceKeys) { }

        public object Get(string key)
        {
            return null;
        }

        public void Remove(string key) { }

        public event CacheEventHandler OnInsert;
        public event CacheEventHandler OnRemove;
    }
}
