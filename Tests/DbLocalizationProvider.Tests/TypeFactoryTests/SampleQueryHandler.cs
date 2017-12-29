using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class SampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public string Execute(SampleQuery query)
        {
            return "Sample string";
        }
    }

    public class DecoratedSampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public DecoratedSampleQueryHandler(SampleQueryHandler inner) { }

        public string Execute(SampleQuery query)
        {
            return "set from decorator";
        }
    }
}
