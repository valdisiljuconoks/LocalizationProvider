using System.Collections.Generic;

namespace DbLocalizationProvider.Import
{
    public class ImportSettings
    {
        public ICollection<IResourceImporter> Providers { get; } = new List<IResourceImporter>
                                                                   {
                                                                       new JsonResourceImporter()
                                                                   };
    }
}
