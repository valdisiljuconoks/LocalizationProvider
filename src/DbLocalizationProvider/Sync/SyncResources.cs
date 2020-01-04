using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    ///     Implement handler for this command when you are asked to synchronize resources from code to storage implementation
    ///     of your choice
    /// </summary>
    public class SyncResources
    {
        /// <summary>
        ///     Implement handler for this command when you are asked to synchronize resources from code to storage implementation
        ///     of your choice
        /// </summary>
        public class Query : IQuery<IEnumerable<LocalizationResource>> { }
    }
}
