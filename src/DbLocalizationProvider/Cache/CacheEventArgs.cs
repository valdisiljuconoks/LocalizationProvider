using System;

namespace DbLocalizationProvider.Cache {
    [Serializable]
    public class CacheEventArgs
    {
        public static readonly CacheEventArgs Empty = new CacheEventArgs(CacheOperation.None, string.Empty, string.Empty);

        public CacheEventArgs(CacheOperation operation, string cacheKey, string resourceKey)
        {
            Operation = operation;
            CacheKey = cacheKey;
            ResourceKey = resourceKey;
        }

        public CacheOperation Operation { get; }

        public string CacheKey { get; }

        public string ResourceKey { get; }
    }
}