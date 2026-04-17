// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Cache;
using EPiServer.Framework.Cache;

namespace DbLocalizationProvider.EPiServer;

internal class EPiServerCache(ISynchronizedObjectInstanceCache epiCache) : ICache
{
    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        epiCache.Insert(key, value, CacheEvictionPolicy.Empty);
    }

    public object Get(string key)
    {
        return epiCache.Get(key);
    }

    public void Remove(string key)
    {
        epiCache.Remove(key);
    }
}
