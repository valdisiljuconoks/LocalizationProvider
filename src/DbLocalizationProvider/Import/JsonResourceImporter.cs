using System.Collections.Generic;
using DbLocalizationProvider.Export;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Import
{
    public class JsonResourceImporter : IResourceImporter
    {
        private static readonly string[] _extensions = { ".json" };

        public string FormatName => "JSON";

        public string[] SupportedFileExtensions => _extensions;

        public string ProviderId => "json";

        public ICollection<LocalizationResource> Parse(string fileContent)
        {
            return JsonConvert.DeserializeObject<ICollection<LocalizationResource>>(fileContent, JsonResourceExporter.DefaultSettings);
        }
    }
}
