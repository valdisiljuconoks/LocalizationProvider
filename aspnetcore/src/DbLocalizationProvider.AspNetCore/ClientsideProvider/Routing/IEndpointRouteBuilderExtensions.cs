// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider.Routing;

/// <summary>
/// Analyzer is happy now
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Use this method if you are relying on ASP.NET Core EndpointRouting
    /// </summary>
    /// <param name="builder">EndpointRoute builder</param>
    /// <param name="path">Path on which you want to map client-side provider</param>
    /// <returns>The same EndpointRoute builder to support API chaining</returns>
    /// <exception cref="ArgumentNullException">If you gave me empty path to map on, nothing else to do here as just throw up</exception>
    public static IEndpointRouteBuilder MapDbLocalizationClientsideProvider(
        this IEndpointRouteBuilder builder,
        string path = "/jsl10n")
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        ClientsideConfigurationContext.SetRootPath(path);

        var pipeline = builder.CreateApplicationBuilder()
            .UseMiddleware<RequestHandler>()
            .Build();

        builder.Map(ClientsideConfigurationContext.RootPath + "/{*remaining}", pipeline);

        return builder;
    }
}
