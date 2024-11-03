// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AdminUI.AspNetCore.Configuration;
using DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;
using DbLocalizationProvider.AdminUI.AspNetCore.Queries;
using DbLocalizationProvider.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

/// <summary>
/// Do I really need to document extension classes? (Making analyzer happy)
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Use this method if you wanna see AdminUI under given path.
    /// </summary>
    /// <param name="app">Whatever</param>
    /// <exception cref="ArgumentNullException">Is thrown if <see cref="UiConfigurationContext.RootUrl" /> is not set.</exception>
    /// <returns>If you want to chain calls further, you can use the same application builder that was used.</returns>
    public static IApplicationBuilder UseDbLocalizationProviderAdminUI(this IApplicationBuilder app)
    {
        var uiConfigurationContext = app.ApplicationServices.GetRequiredService<IOptions<UiConfigurationContext>>();

        var path = uiConfigurationContext.Value.RootUrl;
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        // add checker middleware - to support registration order verification
        app.UseMiddleware<AdminUIMarkerMiddleware>();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(
                typeof(IApplicationBuilderExtensions).Assembly,
                "DbLocalizationProvider.AdminUI.AspNetCore.node_modules._fortawesome.fontawesome_free.webfonts"),
            ServeUnknownFileTypes = true,
            RequestPath = path + "/webfonts"
        });

        var factory = app.ApplicationServices.GetService<TypeFactory>();

        // If Mvc config is added *after* DbLocalizationProvider setup
        // this is a moment when we still can influence things
        // so if available languages should be used from localization options set for requests
        // we can override handler here - but only if it's still well known type
        // if we don't know the handler - this means that somebody override it and basically we can't touch it anymore
        if (factory != null
            && !uiConfigurationContext.Value.UseAvailableLanguageListFromStorage
            && (typeof(AvailableLanguagesHandler).IsAssignableFrom(factory.GetHandlerType<AvailableLanguages.Query>())
                || typeof(AvailableLanguages.Handler).IsAssignableFrom(factory.GetHandlerType<AvailableLanguages.Query>())))
        {
            var requestOptions = app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            factory
                .ForQuery<AvailableLanguages.Query>()
                .SetHandler(() => new AvailableLanguagesHandler(requestOptions.Value.SupportedUICultures));
        }

        // postfix registered import/export providers
        var providerSettings = app.ApplicationServices.GetService<IOptions<ProviderSettings>>();
        if (providerSettings != null)
        {
            var context = app.ApplicationServices.GetRequiredService<IOptions<ConfigurationContext>>();
            foreach (var importer in providerSettings.Value.Importers)
            {
                context.Value.Import.Providers.Add(importer);
            }

            foreach (var exporter in providerSettings.Value.Exporters)
            {
                context.Value.Export.Providers.Add(exporter);
            }
        }

        return app;
    }
}
