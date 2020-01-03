// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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
            var result = JsonConvert.DeserializeObject<ICollection<LocalizationResource>>(fileContent, JsonResourceExporter.DefaultSettings)
                                    .Where(r => r.Translations != null && r.Translations.Count > 0)
                                    .ToList();

            var detectedLanguages = result.SelectMany(r => r.Translations.Select(t => t.Language))
                                          .Distinct()
                                          .Where(l => !string.IsNullOrEmpty(l));

            return new ParseResult(result, detectedLanguages.Select(l => new CultureInfo(l)).ToList());
        }
    }
}
