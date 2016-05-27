using System;
using System.Globalization;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider
{
    [Obsolete("Type will be moved to DbLocalizationProvider.EPiServer namespace on next release")]
    public static class EnumExtensions
    {
        public static string Translate(this Enum target, params object[] formatArguments)
        {
            var resourceKey = $"{target.GetType().FullName}.{target}";
            return LocalizationService.Current.GetStringByCulture(resourceKey, CultureInfo.CurrentUICulture, formatArguments);
        }
    }
}
