using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.EPiServer.Sample
{
    public class SomeManualResources : IManualResourceProvider
    {
        public IEnumerable<ManualResource> GetResources()
        {
            return new List<ManualResource> { new ManualResource("Manual.Resource.1", "Invariant", CultureInfo.InvariantCulture) };
        }
    }
}
