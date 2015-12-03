using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Localization;

namespace TechFellow.DbLocalizationProvider
{
    public class DatabaseLocalizationProvider : LocalizationProvider
    {
        public override IEnumerable<CultureInfo> AvailableLanguages { get; }

        public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return null;
        }

        public override IEnumerable<ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return null;
        }
    }
}
