using System.Collections.Generic;
using System.Collections.Specialized;

namespace DbLocalizationProvider.Export
{
    public interface IResourceExporter
    {
        string FormatName { get; }

        string ProviderId { get; }

        ExportResult Export(ICollection<LocalizationResource> resources, NameValueCollection parameters);
    }
}
