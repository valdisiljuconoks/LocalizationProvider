using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync
{
    /// <summary>
    /// Command to be executed when storage implementation is requested to get its affairs in order and initialize data structures if needed
    /// </summary>
    public class UpdateSchema
    {
        /// <summary>
        /// Command definition itself
        /// </summary>
        public class Command : ICommand { }
    }
}
