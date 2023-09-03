using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class SampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public Task<string> Execute(SampleQuery query)
        {
            return Task.FromResult("Sample string");
        }
    }
}
