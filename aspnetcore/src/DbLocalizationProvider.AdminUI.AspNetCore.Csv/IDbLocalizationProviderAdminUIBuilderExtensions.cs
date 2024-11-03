// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.AdminUI.AspNetCore.Configuration;
using DbLocalizationProvider.Csv;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace DbLocalizationProvider.AdminUI.AspNetCore;

/// <summary>
/// Do I really need to document extension classes?
/// </summary>
public static class IDbLocalizationProviderAdminUIBuilderExtensions
{
    /// <summary>
    /// Adds support for export and import in Csv format
    /// </summary>
    /// <param name="builder">Service collection</param>
    /// <returns></returns>
    public static IDbLocalizationProviderAdminUIBuilder AddCsvSupport(
        this IDbLocalizationProviderAdminUIBuilder builder)
    {
        builder.Services.Configure<ProviderSettings>(s =>
        {
            s.Exporters.Add(new CsvResourceExporter());
            s.Importers.Add(new CsvResourceFormatParser());
        });

        return builder;
    }
}
