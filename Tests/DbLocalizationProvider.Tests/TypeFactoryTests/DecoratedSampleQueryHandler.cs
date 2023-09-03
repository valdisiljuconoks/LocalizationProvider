using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class DecoratedSampleQueryHandler : IQueryHandler<SampleQuery, string>
    {
        public DecoratedSampleQueryHandler(SampleQueryHandler inner) { }

        public Task<string> Execute(SampleQuery query)
        {
            return Task.FromResult("set from decorator");
        }
    }
}
