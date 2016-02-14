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
            var data = base.CreateMetadata(attributes,
                                           containerType,
                                           modelAccessor,
                                           modelType,
                                           propertyName);

            if (containerType == null)
            {
                return data;
            }

            var resourceKey = $"{containerType.FullName}.{propertyName}";
            if (ConfigurationContext.Current.EnableLocalization())
            {
                data.DisplayName = resourceKey;
            }
            else
            {
                var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
                data.DisplayName = localizationService.GetString(resourceKey);
            }

            return data;
        }
    }
}
