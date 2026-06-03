// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;

namespace DbLocalizationProvider;

/// <summary>
/// Extension methods for <see cref="ConfigurationContext" />
/// </summary>
public static class ConfigurationContextExtensions
{
    /// <summary>
    /// Performs resource lookup by given key.
    /// Use with caution. This is optimization workaround for the cases when you need to filter out and allow some of the resources to pass-through
    /// for <see cref="Queries.GetTranslation.Query" /> query.
    /// </summary>
    /// <param name="context">ConfigurationContext</param>
    /// <param name="resourceKey">Resource key</param>
    /// <remarks>Returns <c>true</c> if you want to continue translation lookup for given resource key; otherwise <c>false</c>.</remarks>
    public static bool ShouldLookupResource(this ConfigurationContext context, string? resourceKey)
    {
        if (resourceKey == null)
        {
            return false;
        }

        return context.ResourceLookupFilter?.Invoke(resourceKey)
               ?? (!resourceKey.StartsWith("/", StringComparison.OrdinalIgnoreCase) || context.EnableLegacyMode());
    }
}
