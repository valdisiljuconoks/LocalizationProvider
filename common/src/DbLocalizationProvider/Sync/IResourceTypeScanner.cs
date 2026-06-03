// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync;

/// <summary>
/// Defines methods for scanning resource types.
/// </summary>
public interface IResourceTypeScanner
{
    /// <summary>
    /// Determines whether the specified type should be scanned.
    /// </summary>
    /// <param name="target">The type to check.</param>
    /// <returns><c>true</c> if the type should be scanned; otherwise, <c>false</c>.</returns>
    bool ShouldScan(Type target);

    /// <summary>
    /// Gets the resource key prefix for the specified type.
    /// </summary>
    /// <param name="target">The type for which to get the resource key prefix.</param>
    /// <param name="keyPrefix">An optional key prefix.</param>
    /// <returns>The resource key prefix.</returns>
    string? GetResourceKeyPrefix(Type target, string? keyPrefix = null);

    /// <summary>
    /// Gets the class-level resources for the specified type.
    /// </summary>
    /// <param name="target">The type for which to get the class-level resources.</param>
    /// <param name="resourceKeyPrefix">The resource key prefix.</param>
    /// <returns>A collection of discovered resources.</returns>
    ICollection<DiscoveredResource> GetClassLevelResources(Type target, string resourceKeyPrefix);

    /// <summary>
    /// Gets the resources for the specified type.
    /// </summary>
    /// <param name="target">The type for which to get the resources.</param>
    /// <param name="resourceKeyPrefix">The resource key prefix.</param>
    /// <returns>A collection of discovered resources.</returns>
    ICollection<DiscoveredResource> GetResources(Type target, string resourceKeyPrefix);
}
