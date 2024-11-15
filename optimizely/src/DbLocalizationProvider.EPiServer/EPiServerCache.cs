// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Cache;
using EPiServer;

namespace DbLocalizationProvider.EPiServer;

internal class EPiServerCache : ICache
{
    public void Insert(string key, object value, bool insertIntoKnownResourceKeys)
    {
        CacheManager.Insert(key, value);
    }

    public object Get(string key)
    {
        return CacheManager.Get(key);
    }

    public void Remove(string key)
    {
        CacheManager.Remove(key);
    }
}
