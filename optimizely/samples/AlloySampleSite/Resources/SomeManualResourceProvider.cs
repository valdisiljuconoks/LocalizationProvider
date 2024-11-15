using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Sync;
using EPiServer.DataAbstraction;

namespace AlloySampleSite.Resources
{
    public class SomeManualResourceProvider : IManualResourceProvider
    {
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public SomeManualResourceProvider(ILanguageBranchRepository languageBranchRepository)
        {
            _languageBranchRepository = languageBranchRepository;
        }

        public IEnumerable<ManualResource> GetResources()
        {
            return new List<ManualResource> { new("Some manual resource", "Some manual resource", CultureInfo.InvariantCulture) };
        }
    }
}
