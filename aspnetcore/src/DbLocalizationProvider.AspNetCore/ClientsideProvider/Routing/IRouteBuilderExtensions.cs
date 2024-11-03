// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;

/// <summary>
/// Static class
/// </summary>
public static class IRouteBuilderExtensions
{
    /// <summary>
    /// Maps the localization clientside provider on specified path.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="path">The path to map on.</param>
    /// <returns>Route builder to support API call chaining</returns>
    /// <exception cref="ArgumentNullException">path</exception>
    public static IRouteBuilder MapDbLocalizationClientsideProvider(this IRouteBuilder builder, string path = "/jsl10n")
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        ClientsideConfigurationContext.SetRootPath(path);

        builder.MapMiddlewareRoute(path + "/{*remaining}", b => b.UseMiddleware<RequestHandler>());

        return builder;
    }
}
