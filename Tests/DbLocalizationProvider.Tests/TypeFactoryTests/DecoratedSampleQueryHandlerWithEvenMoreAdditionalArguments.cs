using DbLocalizationProvider.Abstractions;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Tests.TypeFactoryTests;

public class DecoratedSampleQueryHandlerWithEvenMoreAdditionalArguments : IQueryHandler<SampleQuery, string>
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly SomeSimpleDependency _simpleDependency;

    public DecoratedSampleQueryHandlerWithEvenMoreAdditionalArguments(
        SampleQueryHandler inner,
        SomeSimpleDependency simpleDependency,
        IOptions<ConfigurationContext> configurationContext)
    {
        _simpleDependency = simpleDependency;
        _configurationContext = configurationContext;
    }

    public string Execute(SampleQuery query)
    {
        return $"set from decorator. from context: {_configurationContext.Value.DiagnosticsEnabled}";
    }
}

public class SomeSimpleDependency { }
