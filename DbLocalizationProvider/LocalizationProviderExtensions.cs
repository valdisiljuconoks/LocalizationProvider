using System;
using System.Globalization;
using System.Linq.Expressions;

namespace DbLocalizationProvider
{
    public static class LocalizationProviderExtensions
    {
        public static string GetString(this LocalizationProvider service, Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(service, resource, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationProvider service, Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            return service.GetStringByCulture(resource, culture, formatArguments);
        }
        
    }
}
