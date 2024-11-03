// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.AspNetCore.Cache;

/// <summary>
/// Clears the cache.
/// </summary>
/// <param name="cache">Cache implementation</param>
public class ClearCacheHandler(ICacheManager cache) : ICommandHandler<ClearCache.Command>
{
    /// <inheritdoc />
    public void Execute(ClearCache.Command command)
    {
        foreach (var itemToRemove in cache.Keys)
        {
            cache.Remove(itemToRemove);
        }
    }
}
