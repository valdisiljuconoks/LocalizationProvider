using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace DbLocalizationProvider.DataAnnotations
{
    public class LocalizedModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            if(metadata.ContainerType == null)
            {
                return base.GetValidators(metadata, context, attributes);
            }

            foreach (var attribute in attributes.OfType<ValidationAttribute>())
            {
                var resourceKey = $"{metadata.ContainerType.FullName}.{metadata.PropertyName}-{attribute.GetType().Name.Replace("Attribute", string.Empty)}";
                attribute.ErrorMessage = ModelMetadataLocalizationHelper.GetValue(resourceKey);
            }

            return base.GetValidators(metadata, context, attributes);
        }
    }
}
