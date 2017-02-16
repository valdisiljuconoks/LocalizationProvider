using System;
using System.Globalization;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider
{
    public static class EnumExtensions
    {
        public static string Translate(this Enum target, params object[] formatArguments)
        {
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(target.GetType(), target.ToString());
            return LocalizationProvider.Current.GetStringByCulture(resourceKey, CultureInfo.CurrentUICulture, formatArguments);
        }
    }
}
