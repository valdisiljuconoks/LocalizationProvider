// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

/// <inheritdoc />
public class DbLocalizationProviderBuilder : IDbLocalizationProviderAdminUIBuilder
{
    /// <summary>
    /// Creates new instance of builder.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="context">Configuration context.</param>
    public DbLocalizationProviderBuilder(IServiceCollection services, UiConfigurationContext context)
    {
        Services = services;
        UiContext = context;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public UiConfigurationContext UiContext { get; }
}
