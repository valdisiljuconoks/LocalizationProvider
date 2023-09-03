using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    public class NorwegianDefaultCulture : IQueryHandler<DetermineDefaultCulture.Query, string>
    {
        public Task<string> Execute(DetermineDefaultCulture.Query query)
        {
            return Task.FromResult("no");
        }
    }
}
