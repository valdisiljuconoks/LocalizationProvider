using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.EPiServer;

namespace DbLocalizationProvider
{
    [Obsolete("With next version type will be moved to DbLocalizationProvider.EPiServer namespace")]
    public class DatabaseLocalizationProvider : global::EPiServer.Framework.Localization.LocalizationProvider
    {
        private readonly LocalizationProvider _inner;

        public DatabaseLocalizationProvider()
        {
            _inner = new LocalizationProvider(new EPiServerCacheManager());
        }

        public override IEnumerable<CultureInfo> AvailableLanguages => _inner.AvailableLanguages;

        public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return _inner.GetString(originalKey, culture);
        }

        public override IEnumerable<global::EPiServer.Framework.Localization.ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return _inner.GetAllStrings(originalKey, culture)
                         .Select(r => new global::EPiServer.Framework.Localization.ResourceItem(r.Key, r.Value, r.SourceCulture));
        }
    }
}
