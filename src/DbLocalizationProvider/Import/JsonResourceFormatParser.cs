using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var detectedLanguages = result.SelectMany(r => r.Translations.Select(t => t.Language))
                .Distinct()
                .Where(l => !string.IsNullOrEmpty(l));

            return new ParseResult(result, detectedLanguages.Select(l => new CultureInfo(l)).ToList());
        }
    }
}
