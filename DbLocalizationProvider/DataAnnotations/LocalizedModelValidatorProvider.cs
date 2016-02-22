using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace DbLocalizationProvider.DataAnnotations
{
    public class LocalizedModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            if (metadata.ContainerType == null)
            {
                return base.GetValidators(metadata, context, attributes);
            }

            foreach (var attribute in attributes.OfType<ValidationAttribute>())
            {
                var resourceKey = $"{metadata.ContainerType.FullName}.{metadata.PropertyName}-{attribute.GetType().Name.Replace("Attribute", string.Empty)}";

                if (ConfigurationContext.Current.EnableLocalization())
                {
                    var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
                    var localizedErrorMessage = localizationService.GetString(resourceKey);

                    attribute.ErrorMessage = localizedErrorMessage ?? attribute.FormatErrorMessage(metadata.DisplayName);
                }
                else
                {
                    attribute.ErrorMessage = resourceKey;
                }
            }

            return base.GetValidators(metadata, context, attributes);
        }
    }
}
