using System;
using System.Globalization;
using System.Linq.Expressions;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    [Obsolete("With next version type will be moved to DbLocalizationProvider.EPiServer namespace")]
    public static class LocalizationServiceExtensions
    {
        public static string GetString(this LocalizationService service, Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(service, resource, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationService service, Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            return GetStringByCulture(service, resourceKey, culture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationService service, string resourceKey, CultureInfo culture, params object[] formatArguments)
        {
            return LocalizationProvider.Current.GetStringByCulture(resourceKey, culture, formatArguments);
        }
    }
}
