using System;
using System.Globalization;

namespace DbLocalizationProvider
{
    public static class EnumExtensions
    {
        public static string Translate(this Enum target, params object[] formatArguments)
        {
            var resourceKey = $"{target.GetType().FullName}.{target}";
            return LocalizationProvider.Current.GetStringByCulture(resourceKey, CultureInfo.CurrentUICulture, formatArguments);
        }
    }
}
