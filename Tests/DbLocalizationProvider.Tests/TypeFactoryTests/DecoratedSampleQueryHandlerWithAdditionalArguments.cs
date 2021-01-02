using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class DecoratedSampleQueryHandlerWithAdditionalArguments : IQueryHandler<SampleQuery, string>
    {
        private readonly ConfigurationContext _configurationContext;

        public DecoratedSampleQueryHandlerWithAdditionalArguments(
            SampleQueryHandler inner,
            ConfigurationContext configurationContext)
        {
            _configurationContext = configurationContext;
        }

        public string Execute(SampleQuery query)
        {
            return $"set from decorator. from context: {_configurationContext.DiagnosticsEnabled}";
        }
    }
}
