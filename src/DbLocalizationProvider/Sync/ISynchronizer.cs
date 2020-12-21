using System.Collections.Generic;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// Ensures that resources from code and/or manually crafted are pushed down to underlying storage.
    /// </summary>
    public interface ISynchronizer
    {
        /// <summary>
        /// Synchronizes manually crafted resources
        /// </summary>
        /// <param name="resources"></param>
        void RegisterManually(IEnumerable<ManualResource> resources);
    }
}
