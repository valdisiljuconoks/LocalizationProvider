using System.Collections.Generic;

namespace DbLocalizationProvider.Export
{
    public class ExportProviders
    {
        public static ICollection<IResourceExporter> Providers { get; } = new List<IResourceExporter>
                                                                          {
                                                                              new JsonResourceExporter()
                                                                          };
    }
}
