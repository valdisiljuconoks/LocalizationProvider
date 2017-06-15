namespace DbLocalizationProvider.Cache
{
    public class CacheKeyHelper
    {
        public const string CacheKeyPrefix = "DbLocalizationProviderCache";

        public static string BuildKey(string key)
        {
            return $"{CacheKeyPrefix}_{key}";
        }

        public static string GetResourceKeyFromCacheKey(string cacheKey)
        {
            return cacheKey.Replace($"{CacheKeyPrefix}_", string.Empty);
        }
    }
}
