// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Builder;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Extension to initialize and setup provider.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Synchronizes resources with underlying storage
    /// </summary>
    /// <param name="builder">ASP.NET Core application builder</param>
    /// <returns>ASP.NET Core application builder to enable fluent API call chains</returns>
    public static IApplicationBuilder UseDbLocalizationProvider(this IApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.ApplicationServices.UseDbLocalizationProvider();

        return builder;
    }
}
