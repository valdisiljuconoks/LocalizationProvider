// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Cache
{
    /// <summary>
    /// Event handler signature for those whole are handling cache events.
    /// </summary>
    /// <param name="e">The <see cref="CacheEventArgs"/> instance containing the event data.</param>
    public delegate void CacheEventHandler(CacheEventArgs e);
}
