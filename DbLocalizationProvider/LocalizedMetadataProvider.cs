using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider
{
    public class LocalizedMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            var data = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            if (containerType == null)
            {
                return data;
            }

            var resourceKey = $"{containerType.FullName}.{propertyName}";
            if (ConfigurationContext.Current.EnableLocalization())
            {
                var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
                var localizedDisplayName = localizationService.GetString(resourceKey);
                data.DisplayName = localizedDisplayName;

                if (ConfigurationContext.Current.EnableLegacyMode())
                {
                    // for the legacy purposes - we need to look for this resource value as resource translation
                    // once again - this will make sure that existing XPath resources are still working
                    if (localizedDisplayName.StartsWith("/"))
                    {
                        data.DisplayName = localizationService.GetString(localizedDisplayName);
                    }
                }
            }
            else
            {
                data.DisplayName = resourceKey;
            }

            return data;
        }
    }
}
