using System.Collections.Generic;

namespace DbLocalizationProvider.Export
{
    public interface IResourceExporter
    {
        string FormatName { get; }

        string ProviderId { get; }

        ExportResult Export(ICollection<LocalizationResource> resources);
    }
}
