using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class SampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public string Execute(SampleQuery query)
        {
            return "Sample string";
        }
    }
}