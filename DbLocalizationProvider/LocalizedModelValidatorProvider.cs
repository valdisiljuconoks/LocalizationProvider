using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace DbLocalizationProvider
{
    public class LocalizedModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            if (metadata.ContainerType != null)
            {
                foreach (var attribute in attributes.OfType<ValidationAttribute>())
                {
                    if (ConfigurationContext.Current.EnableLocalization()) { }

                    //attribute.ErrorMessage = metadata.ContainerType.Translate(GetKey(metadata, attribute),
                    //                                                          attribute.ErrorMessage ?? attribute.FormatErrorMessage(metadata.DisplayName));
                }
            }

            return base.GetValidators(metadata, context, attributes);
        }

        private static string GetKey(ModelMetadata metadata, ValidationAttribute attribute)
        {
            return metadata.PropertyName + "." + attribute.GetType().Name.Replace("Attribute", "ValidationMessage");
        }
    }
}
