using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class DecoratedSampleQueryHandlerWithEvenMoreAdditionalArguments : IQueryHandler<SampleQuery, string>
    {
        private readonly SomeSimpleDependency _simpleDependency;
        private readonly ConfigurationContext _configurationContext;

        public DecoratedSampleQueryHandlerWithEvenMoreAdditionalArguments(
            SampleQueryHandler inner,
            SomeSimpleDependency simpleDependency,
            ConfigurationContext configurationContext)
        {
            _simpleDependency = simpleDependency;
            _configurationContext = configurationContext;
        }

        public string Execute(SampleQuery query)
        {
            return $"set from decorator. from context: {_configurationContext.DiagnosticsEnabled}";
        }
    }

    public class SomeSimpleDependency
    {

    }
}
