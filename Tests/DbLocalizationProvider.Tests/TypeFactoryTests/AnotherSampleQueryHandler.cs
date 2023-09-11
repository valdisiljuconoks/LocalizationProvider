using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests;

public class AnotherSampleQueryHandler : IQueryHandler<SampleQuery, string>
{
    public string Execute(SampleQuery query)
    {
        return "Another sample string";
    }
}
