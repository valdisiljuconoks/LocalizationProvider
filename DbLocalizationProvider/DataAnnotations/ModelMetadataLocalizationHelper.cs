using System;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.DataAnnotations
{
    internal class ModelMetadataLocalizationHelper
    {
        internal static string GetTranslation(string resourceKey)
        {
            var result = resourceKey;
            if(!ConfigurationContext.Current.EnableLocalization())
            {
                return result;
            }

            var localizedDisplayName = LocalizationProvider.Current.GetString(resourceKey);
            result = localizedDisplayName;

            if(!ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode())
                return result;

            // for the legacy purposes - we need to look for this resource value as resource translation
            // once again - this will make sure that existing XPath resources are still working
            if(localizedDisplayName.StartsWith("/"))
            {
                result = LocalizationProvider.Current.GetString(localizedDisplayName);
            }

            return result;
        }

        internal static string GetTranslation(Type containerType, string propertyName)
        {
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(containerType, propertyName);
            return GetTranslation(resourceKey);
        }
    }
}
