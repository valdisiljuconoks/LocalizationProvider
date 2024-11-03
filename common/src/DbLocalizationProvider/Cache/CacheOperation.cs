// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Cache;

/// <summary>
/// Indicates what operation has been performed in cache.
/// </summary>
public enum CacheOperation
{
    /// <summary>
    /// Not specified
    /// </summary>
    None,

    /// <summary>
    /// Item has been added tot the cache
    /// </summary>
    Insert,

    /// <summary>
    /// Item has been removed from the cache
    /// </summary>
    Remove
}
