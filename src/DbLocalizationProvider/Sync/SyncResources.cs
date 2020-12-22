using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// Implement handler for this command when you are asked to synchronize resources from code to storage implementation
    /// of your choice
    /// </summary>
    public class SyncResources
    {
        /// <summary>
        /// Implement handler for this command when you are asked to synchronize resources from code to storage implementation
        /// of your choice
        /// </summary>
        public class Query : IQuery<IEnumerable<LocalizationResource>>
        {
            /// <summary>
            /// Creates new instance of sync query class.
            /// </summary>
            /// <param name="discoveredResources">List of discovered localized resources (filled automatically by library)</param>
            /// <param name="discoveredModels">List of discovered localized models (filled automatically by library)</param>
            public Query(ICollection<DiscoveredResource> discoveredResources, ICollection<DiscoveredResource> discoveredModels)
            {
                DiscoveredResources = discoveredResources;
                DiscoveredModels = discoveredModels;
            }

            /// <summary>
            /// List of discovered localized resources
            /// </summary>
            public ICollection<DiscoveredResource> DiscoveredResources { get; }

            /// <summary>
            /// List of discovered localized models
            /// </summary>
            public ICollection<DiscoveredResource> DiscoveredModels { get; }
        }
    }
}
