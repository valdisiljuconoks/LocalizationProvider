using DbLocalizationProvider.Abstractions;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.Tests.TypeFactoryTests;

public class DecoratedSampleQueryHandlerWithAdditionalArguments : IQueryHandler<SampleQuery, string>
{
    private readonly ConfigurationContext _configurationContext;

    public DecoratedSampleQueryHandlerWithAdditionalArguments(
        SampleQueryHandler inner,
        IOptions<ConfigurationContext> configurationContext)
    {
        _configurationContext = configurationContext.Value;
    }

    public string Execute(SampleQuery query)
    {
        return $"set from decorator. from context: {_configurationContext.DiagnosticsEnabled}";
    }
}
