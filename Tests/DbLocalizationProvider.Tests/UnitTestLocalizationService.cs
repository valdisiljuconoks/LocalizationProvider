using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider.Tests
{
    internal class UnitTestLocalizationService : LocalizationService
    {
        private readonly string _resourceValue;

        public UnitTestLocalizationService(string resourceValue) : this(null, resourceValue) { }

        public UnitTestLocalizationService(ResourceKeyHandler keyHandler, string resourceValue) : base(keyHandler)
        {
            _resourceValue = resourceValue;
        }

        public override IEnumerable<CultureInfo> AvailableLocalizations => Enumerable.Empty<CultureInfo>();

        public override string GetStringByCulture(string resourceKey, FallbackBehaviors fallbackBehavior, CultureInfo culture)
        {
            return _resourceValue;
        }

        protected override string LoadString(string[] normalizedKey, string originalKey, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ResourceItem> GetAllStringsByCulture(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
