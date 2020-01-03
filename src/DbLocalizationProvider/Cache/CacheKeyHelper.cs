// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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
