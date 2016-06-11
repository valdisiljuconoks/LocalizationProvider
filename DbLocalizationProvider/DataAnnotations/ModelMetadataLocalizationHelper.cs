using System;
using System.ComponentModel.DataAnnotations;

namespace DbLocalizationProvider.DataAnnotations
{
    internal class ModelMetadataLocalizationHelper
    {
        internal static string GetValue(string resourceKey)
        {
            var result = resourceKey;
            if(!ConfigurationContext.Current.EnableLocalization())
            {
                return result;
            }

            var localizedDisplayName = LocalizationProvider.Current.GetString(resourceKey);
            result = localizedDisplayName;

            if(ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode())
            {
                // for the legacy purposes - we need to look for this resource value as resource translation
                // once again - this will make sure that existing XPath resources are still working
                if(localizedDisplayName.StartsWith("/"))
                {
                    result = LocalizationProvider.Current.GetString(localizedDisplayName);
                }
            }

            return result;
        }

        internal static string GetValue(Type containerType, string propertyName)
        {
            var resourceKey = $"{containerType.FullName}.{propertyName}";
            return GetValue(resourceKey);
        }

        internal static string BuildResourceKey(string keyPrefix, ValidationAttribute attribute)
        {
            var result = $"{keyPrefix}-{attribute.GetType().Name.Replace("Attribute", string.Empty)}";
            if(attribute.GetType().IsAssignableFrom(typeof(DataTypeAttribute)))
            {
                result += ((DataTypeAttribute) attribute).DataType;
            }

            return result;
        }
    }
}
