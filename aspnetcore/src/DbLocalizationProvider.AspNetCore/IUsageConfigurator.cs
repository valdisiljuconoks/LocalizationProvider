// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Configurator called just before the service usage. This is an option for specific runtime implementation to configure stuff.
/// </summary>
public interface IUsageConfigurator
{
    /// <summary>
    /// Callback to influence configuration content and do some pre-usage magic (if needed).
    /// </summary>
    /// <param name="context">Configuration context.</param>
    /// <param name="serviceProvider">Service provider.</param>
    public void Configure(IOptions<ConfigurationContext> context, IServiceProvider serviceProvider);
}
