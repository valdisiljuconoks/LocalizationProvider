// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider.Cache;

/// <summary>
/// Arguments for those who are interested in cache events
/// </summary>
[Serializable]
public class CacheEventArgs
{
    /// <summary>
    /// Empty args. Nothing to see here.
    /// </summary>
    public static readonly CacheEventArgs Empty = new(CacheOperation.None, string.Empty, string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheEventArgs" /> class.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="resourceKey">The resource key.</param>
    public CacheEventArgs(CacheOperation operation, string cacheKey, string resourceKey)
    {
        Operation = operation;
        CacheKey = cacheKey;
        ResourceKey = resourceKey;
    }

    /// <summary>
    /// Gets the operation.
    /// </summary>
    public CacheOperation Operation { get; }

    /// <summary>
    /// Gets the cache key.
    /// </summary>
    public string CacheKey { get; }

    /// <summary>
    /// Gets the resource key.
    /// </summary>
    public string ResourceKey { get; }
}
