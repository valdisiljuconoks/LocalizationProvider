using System;
using System.Globalization;
using System.Linq.Expressions;

namespace DbLocalizationProvider
{
    public static class LocalizationProviderExtensions
    {
        public static string GetString(this LocalizationProvider provider, Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(provider, resource, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationProvider provider, Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            return provider.GetStringByCulture(resource, culture, formatArguments);
        }
    }
}
