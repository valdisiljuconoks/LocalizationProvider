using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

            if(metadata.ContainerType.GetCustomAttribute<LocalizedModelAttribute>() == null)
            {
                return base.GetValidators(metadata, context, attributes);
            }

            foreach (var attribute in attributes.OfType<ValidationAttribute>())
            {
                var resourceKey = ModelMetadataLocalizationHelper.BuildResourceKey($"{metadata.ContainerType.FullName}.{metadata.PropertyName}", attribute);
                var translation = ModelMetadataLocalizationHelper.GetValue(resourceKey);
                if(!string.IsNullOrEmpty(translation))
                {
                    attribute.ErrorMessage = translation;
                }
            }

            return base.GetValidators(metadata, context, attributes);
        }
    }
}
