using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests
{
    public class NorwegianDefaultCulture : IQueryHandler<DetermineDefaultCulture.Query, string>
    {
        public string Execute(DetermineDefaultCulture.Query query)
        {
            return "no";
        }
    }
}
