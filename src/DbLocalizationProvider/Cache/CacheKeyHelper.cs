// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Cache
{
    /// <summary>
    /// Helper is here to save you when you have to deal with cache keys - either build one or deconstruct from resource key.
    /// </summary>
    public class CacheKeyHelper
    {
        /// <summary>
        /// To avoid collisions somehow cache keys need to be prefixed. This is the one.
        /// </summary>
        public const string CacheKeyPrefix = "DbLocalizationProviderCache";

        /// <summary>
        /// Builds the key from resource key.
        /// </summary>
        /// <param name="key">The resource key.</param>
        /// <returns>Cache key</returns>
        public static string BuildKey(string key)
        {
            return $"{CacheKeyPrefix}_{key}";
        }

        /// <summary>
        /// Gets the resource key from cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>Resource key</returns>
        public static string GetResourceKeyFromCacheKey(string cacheKey)
        {
            return cacheKey.Replace($"{CacheKeyPrefix}_", string.Empty);
        }
    }
}
