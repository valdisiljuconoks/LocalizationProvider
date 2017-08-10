using System;
using System.Collections.Generic;
using System.Globalization;

namespace DbLocalizationProvider.Import
{
    public class ParseResult
    {
        public ParseResult(ICollection<LocalizationResource> resources, ICollection<CultureInfo> detectedLanguages)
        {
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
            DetectedLanguages = detectedLanguages ?? throw new ArgumentNullException(nameof(detectedLanguages));
        }

        public ICollection<LocalizationResource> Resources { get; }

        public ICollection<CultureInfo> DetectedLanguages { get; }
    }
}
