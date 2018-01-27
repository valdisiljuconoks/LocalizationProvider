using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore
{
    public class DbStringLocalizer : IStringLocalizer
    {
        private readonly CultureInfo _culture;

        public DbStringLocalizer()
        {
            _culture = CultureInfo.CurrentUICulture;
        }

        public DbStringLocalizer(CultureInfo culture) : this()
        {
            _culture = culture;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new DbStringLocalizer(culture);
        }

        LocalizedString IStringLocalizer.this[string name]
        {
            get
            {
                var value = LocalizationProvider.Current.GetStringByCulture(name, _culture);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        LocalizedString IStringLocalizer.this[string name, params object[] arguments]
        {
            get
            {
                var value = LocalizationProvider.Current.GetStringByCulture(name, _culture, arguments);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }
    }
}
