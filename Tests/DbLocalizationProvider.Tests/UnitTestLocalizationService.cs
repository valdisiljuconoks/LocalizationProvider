using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider.Tests
{
    internal class UnitTestLocalizationService : LocalizationProvider
    {
        private readonly string _resourceValue;

        public UnitTestLocalizationService(string resourceValue)
        {
            _resourceValue = resourceValue;
        }

        public  IEnumerable<CultureInfo> AvailableLocalizations => Enumerable.Empty<CultureInfo>();

        public  string GetStringByCulture(string resourceKey, FallbackBehaviors fallbackBehavior, CultureInfo culture)
        {
            return _resourceValue;
        }

        public override string GetStringByCulture(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            return Format(_resourceValue, formatArguments);
        }

        protected  string LoadString(string[] normalizedKey, string originalKey, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected  IEnumerable<EPiServer.Framework.Localization.ResourceItem> GetAllStringsByCulture(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
