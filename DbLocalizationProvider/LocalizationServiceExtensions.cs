using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Framework.Localization;
using EPiServer.Logging;

namespace DbLocalizationProvider
{
    public static class LocalizationServiceExtensions
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(LocalizationServiceExtensions));

        public static string GetString(this LocalizationService service, Expression<Func<object>> resource, params object[] formatArguments)
        {
            return GetStringByCulture(service, resource, CultureInfo.CurrentUICulture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationService service, Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            if(resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            return GetStringByCulture(service, resourceKey, culture, formatArguments);
        }

        public static string GetStringByCulture(this LocalizationService service, string resourceKey, CultureInfo culture, params object[] formatArguments)
        {
            var resourceValue = service.GetStringByCulture(resourceKey, culture);

            if(formatArguments == null || !formatArguments.Any())
            {
                return resourceValue;
            }

            try
            {
                // check if first element is not scalar - format with named placeholders
                var first = formatArguments.First();
                resourceValue = !PrimitiveTypes.IsPrimitive(first.GetType())
                                    ? Format(resourceValue, first)
                                    : string.Format(resourceValue, formatArguments);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to format resource with key `{resourceKey}`", e);
            }

            return resourceValue;
        }

        internal static string Format(string message, object placeholders)
        {
            var type = placeholders.GetType();
            if(type == typeof(string))
            {
                return string.Format(message, placeholders);
            }

            var properties = type.GetProperties();
            var result = message;

            foreach (var propertyInfo in properties)
            {
                var val = propertyInfo.GetValue(placeholders);
                if(val != null)
                {
                    result = result.Replace($"{{{propertyInfo.Name}}}", val.ToString());
                }
            }

            return result;
        }
    }
}
