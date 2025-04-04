// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

namespace DbLocalizationProvider.Abstractions;

/// <summary>
/// Source type to sync resources from.
/// </summary>
public enum SyncSource
{
    /// <summary>
    /// No one knows how this got into the source code
    /// </summary>
    None = 0,

    /// <summary>
    /// Localized resources (ordinary classes with properties and stuff).
    /// </summary>
    Resources = 1,

    /// <summary>
    /// View models.
    /// </summary>
    Models = 2
}
