using System.Collections.Generic;

namespace DbLocalizationProvider.Import
{
    public interface IResourceImporter
    {
        string FormatName { get; }

        string[] SupportedFileExtensions { get; }

        string ProviderId { get; }

        ICollection<LocalizationResource> Parse(string fileContent);
    }
}
