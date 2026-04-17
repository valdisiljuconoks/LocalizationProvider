// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;
using DbLocalizationProvider.AdminUI.AspNetCore.Security;
using DbLocalizationProvider.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbLocalizationProvider.AdminUI.AspNetCore;

/// <summary>
/// Do I really need to document extension classes?
/// </summary>
public static class IServiceCollectionExtensions
{
    public const string HostAreaName = "4D5A2189D188417485BF6C70546D34A1";
    
    /// <summary>
    /// Adds the DbLocalizationProvider AdminUI to the specified builder.
    /// </summary>
    /// <param name="builder">The DbLocalizationProvider builder.</param>
    /// <param name="setup">An optional action to configure the UI.</param>
    /// <returns>The AdminUI builder for further configuration.</returns>
    public static IDbLocalizationProviderAdminUIBuilder AddDbLocalizationProviderAdminUI(
        this IDbLocalizationProviderBuilder builder,
        Action<UiConfigurationContext>? setup = null)
    {
        return builder.Services.AddDbLocalizationProviderAdminUI(setup);
    }

    /// <summary>
    /// Use this method if you want to add AdminUI component to your application. This is just a part of the setup. You will also need to mount the
    /// component. Use other method (will leave it up to you to figure out which).
    /// </summary>
    /// <param name="services">Collection of the services (Microsoft approach for DI).</param>
    /// <param name="setup">UI setup context will be passed in, so you can do some customization on that object to influence how AdminUI behaves.</param>
    /// <returns>AdminUI builder - so you can do configuration further.</returns>
    public static IDbLocalizationProviderAdminUIBuilder AddDbLocalizationProviderAdminUI(
        this IServiceCollection services,
        Action<UiConfigurationContext>? setup = null)
    {
        var context = new UiConfigurationContext();
        setup?.Invoke(context);

        services.AddOptions();
        services
            .AddOptions<UiConfigurationContext>()
            .Configure(ctx =>
            {
                ctx.CopyFrom(context);
            });

        // add support for admin ui razor class library pages
        services.Configure<RazorPagesOptions>(x =>
        {
            x.Conventions.AuthorizeAreaPage(HostAreaName, "/AdminUI", AccessPolicy.Name);
            x.Conventions.AddAreaPageRoute(HostAreaName, "/AdminUI", context.RootUrl);

            x.Conventions.AuthorizeAreaPage(HostAreaName, "/AdminUITree", AccessPolicy.Name);
            x.Conventions.AddAreaPageRoute(HostAreaName, "/AdminUITree", context.RootUrl + "/tree");
        });

        if (context.AccessPolicyOptions != null)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AccessPolicy.Name, context.AccessPolicyOptions);
            });
        }
        else
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AccessPolicy.Name,
                                  policy => policy.AddRequirements(new CheckAdministratorsRoleRequirement()));
            });
        }

        return new DbLocalizationProviderBuilder(services, context);
    }

    /// <summary>
    /// Using this method runtime will verify AdminUI setup and will throw up if something is wrong.
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <returns>The same collection to support fluent approach and you could chain further</returns>
    public static IServiceCollection VerifyDbLocalizationProviderAdminUISetup(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IStartupFilter, AdminUIVerificationStartupFilter>());

        return services;
    }
}
