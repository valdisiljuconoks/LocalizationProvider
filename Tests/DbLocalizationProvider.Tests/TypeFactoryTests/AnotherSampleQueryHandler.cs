using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class AnotherSampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public Task<string> Execute(SampleQuery query)
        {
            return Task.FromResult("Another sample string");
        }
    }
}
