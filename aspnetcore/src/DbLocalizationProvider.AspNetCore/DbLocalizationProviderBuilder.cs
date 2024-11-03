using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.AspNetCore;

/// <inheritdoc />
public class DbLocalizationProviderBuilder : IDbLocalizationProviderBuilder
{
    /// <summary>
    /// Creates new instance of builder.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="context">Configuration context.</param>
    public DbLocalizationProviderBuilder(IServiceCollection services, ConfigurationContext context)
    {
        Services = services;
        Context = context;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public ConfigurationContext Context { get; }
}
