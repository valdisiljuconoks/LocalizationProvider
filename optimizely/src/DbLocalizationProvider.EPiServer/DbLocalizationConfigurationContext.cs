// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Cache;

namespace DbLocalizationProvider.EPiServer;

/// <summary>
/// Configuration context for DbLocalizationProvider.EPiServer.
/// </summary>
public class DbLocalizationConfigurationContext
{
    /// <summary>
    /// Allows to override internal cache implementation, if problems with default EPiServer implementation arise.
    /// </summary>
    public ICache InnerCache { get; set; } = new EPiServerCache();
}
