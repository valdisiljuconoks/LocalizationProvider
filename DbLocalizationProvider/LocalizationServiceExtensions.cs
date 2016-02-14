using System;
using System.Globalization;
using System.Linq.Expressions;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    public static class LocalizationServiceExtensions
    {
        public static string GetString(this LocalizationService service, Expression<Func<object>> resource)
        {
            return GetStringByCulture(service, resource, CultureInfo.CurrentUICulture);
        }

        public static string GetStringByCulture(this LocalizationService service, Expression<Func<object>> resource, CultureInfo culture)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            return service.GetStringByCulture(resourceKey, culture);
        }
    }
}
