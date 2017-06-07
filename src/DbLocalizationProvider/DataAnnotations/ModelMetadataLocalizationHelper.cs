using System;
using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.DataAnnotations
{
    internal class ModelMetadataLocalizationHelper
    {
        internal static Func<string, bool> UseLegacyMode =
             x => !String.IsNullOrWhiteSpace(x) && x.StartsWith("/") && ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode();

        internal static string GetTranslation(string resourceKey)
        {
            var result = resourceKey;
            if(!ConfigurationContext.Current.EnableLocalization())
                return result;

            var localizedDisplayName = LocalizationProvider.Current.GetString(resourceKey);
            result = localizedDisplayName;

            if(!ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode())
                return result;

            // for the legacy purposes - we need to look for this resource value as resource translation
            // once again - this will make sure that existing XPath resources are still working
            if(UseLegacyMode(localizedDisplayName))
                result = LocalizationProvider.Current.GetString(localizedDisplayName);

            // If other data annotations exists execept for [Display], an exception is thrown when displayname is ""
            // It should be null to avoid exception as ModelMetadata.GetDisplayName only checks for null and not String.Empty
            return string.IsNullOrWhiteSpace(localizedDisplayName) ? null : result;
        }

        internal static string GetTranslation(Type containerType, string propertyName)
        {
            var resourceKey = ResourceKeyBuilder.BuildResourceKey(containerType, propertyName);
            return GetTranslation(resourceKey);
        }
    }
}
