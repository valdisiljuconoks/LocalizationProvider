using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Sync;

namespace DbLocalizationProvider.Core.AspNetSample.Resources;

public class SomeManualResources : IManualResourceProvider
{
    public IEnumerable<ManualResource> GetResources()
    {
        return new List<ManualResource> { new("Manual.Resource.1", "Invariant translation", CultureInfo.InvariantCulture) };
    }
}
