using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Export;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Import
{
    public class JsonResourceFormatParser : IResourceFormatParser
    {
        private static readonly string[] _extensions = { ".json" };

        public string FormatName => "JSON";

        public string[] SupportedFileExtensions => _extensions;

        public string ProviderId => "json";

        public ParseResult Parse(string fileContent)
        {
            var result = JsonConvert.DeserializeObject<ICollection<LocalizationResource>>(fileContent, JsonResourceExporter.DefaultSettings);
            var languages = new List<CultureInfo>();

            return new ParseResult(result, languages);
        }
    }
}
