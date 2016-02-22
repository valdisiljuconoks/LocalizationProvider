using System;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider.DataAnnotations
{
    internal class ModelMetadataLocalizationHelper
    {
        internal static string GetValue(Type containerType, string propertyName)
        {
            var resourceKey = $"{containerType.FullName}.{propertyName}";
            var result = resourceKey;
            if (!ConfigurationContext.Current.EnableLocalization())
            {
                return result;
            }

            var localizedDisplayName = LocalizationService.Current.GetString(resourceKey);
            result = localizedDisplayName;

            if (ConfigurationContext.Current.EnableLegacyMode())
            {
                // for the legacy purposes - we need to look for this resource value as resource translation
                // once again - this will make sure that existing XPath resources are still working
                if (localizedDisplayName.StartsWith("/"))
                {
                    result = LocalizationService.Current.GetString(localizedDisplayName);
                }
            }

            return result;
        }
    }
}
