namespace DbLocalizationProvider.Cache
{
    public interface ICacheManager
    {
        void Insert(string key, object value);

        object Get(string key);

        void Remove(string key);

        event CacheEventHandler OnInsert;

        event CacheEventHandler OnRemove;
    }
}
