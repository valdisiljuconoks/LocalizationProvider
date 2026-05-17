// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider;

internal class CacheHelper
{
    private const string Separator = "_|_";

    public static string GenerateKey(string filename, string language, bool isDebugMode, bool camelCase)
    {
        return $"{filename}{Separator}{language}__{(isDebugMode ? "debug" : "release")}__{camelCase}";
    }

    public static string? GetContainerName(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        return !key.Contains(Separator) ? null : key.Substring(0, key.IndexOf(Separator, StringComparison.Ordinal));
    }

    public static void CacheManagerOnRemove(CacheEventArgs args, ICacheManager cache)
    {
        List<string>? entriesToRemove = null;

        foreach (var cacheKey in cache.Keys)
        {
            var resourceKey = CacheKeyHelper.GetResourceKeyFromCacheKey(cacheKey);
            var containerName = GetContainerName(resourceKey);

            if (containerName != null
                && args.ResourceKey.StartsWith(containerName, StringComparison.OrdinalIgnoreCase))
            {
                (entriesToRemove ??= new List<string>()).Add(cacheKey);
            }
        }

        if (entriesToRemove == null)
        {
            return;
        }

        foreach (var entry in entriesToRemove)
        {
            cache.Remove(entry);
        }
    }
}
