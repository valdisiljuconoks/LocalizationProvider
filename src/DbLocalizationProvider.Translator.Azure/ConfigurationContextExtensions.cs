// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.Translator.Azure;

/// <summary>
/// Extension method to provide nice way to configure SQL Server as resource storage.
/// </summary>
public static class ConfigurationContextExtensions
{
    /// <summary>
    /// If you can afford SQL Server - this method is for you.
    /// </summary>
    /// <param name="context">The configuration context.</param>
    /// <param name="accessKey">Find this access key somewhere in Azure portal for your Cognitive Service instance.</param>
    /// <param name="region">We need to know in which region you are located at.</param>
    /// <returns>The same configuration context, so you can chain it up.</returns>
    public static ConfigurationContext UseAzureCognitiveServices(
        this ConfigurationContext context,
        string accessKey,
        string region)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(region);

        context.Services
            .AddOptions<CognitiveServicesOptions>()
            .Configure(o =>
            {
                o.AccessKey = accessKey;
                o.Region = region;
            });

        context.TypeFactory.AddTransient<ITranslatorService, CognitiveServiceTranslator>();

        return context;
    }
}
