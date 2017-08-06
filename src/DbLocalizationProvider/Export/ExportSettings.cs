using System.Collections.Generic;

namespace DbLocalizationProvider.Export
{
    public class ExportSettings
    {
        public ICollection<IResourceExporter> Providers { get; } = new List<IResourceExporter>
                                                                   {
                                                                       new JsonResourceExporter()
                                                                   };
    }
}
