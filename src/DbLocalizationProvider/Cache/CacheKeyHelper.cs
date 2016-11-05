namespace DbLocalizationProvider.Cache
{
    public class CacheKeyHelper
    {
        public const string CacheKeyPrefix = "DbLocalizationProviderCache";

        public static string BuildKey(string key)
        {
            return $"{CacheKeyPrefix}_{key}";
        }
    }
}
