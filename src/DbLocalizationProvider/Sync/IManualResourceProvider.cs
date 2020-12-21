using System.Collections.Generic;

namespace DbLocalizationProvider.Sync
{
    public interface IManualResourceProvider
    {
        IEnumerable<ManualResource> GetResources();
    }
}