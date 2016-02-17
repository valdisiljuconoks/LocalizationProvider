using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    public static class LocalizationServiceExtensions
    {
        public static string GetString(this LocalizationService service, Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(service, resource, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationService service, Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            var resourceValue = service.GetStringByCulture(resourceKey, culture);

            if (formatArguments != null && formatArguments.Any())
            {
                resourceValue = string.Format(resourceValue, formatArguments);
            }

            return resourceValue;
        }
    }
}
