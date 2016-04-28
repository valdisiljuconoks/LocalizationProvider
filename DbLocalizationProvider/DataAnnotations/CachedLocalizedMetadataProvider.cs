using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace DbLocalizationProvider.DataAnnotations
{
    /// <summary>
    ///     Credits:
    ///     http://world.episerver.com/Blogs/devabees/Dates/2014/3/Integrating-LocalizationService-with-MVC-DataAnnotations/
    /// </summary>
    public class CachedLocalizedMetadataProvider : CachedDataAnnotationsModelMetadataProvider
    {
        protected override CachedDataAnnotationsModelMetadata CreateMetadataFromPrototype(CachedDataAnnotationsModelMetadata prototype, Func<object> modelAccessor)
        {
            var metadataFromPrototype = base.CreateMetadataFromPrototype(prototype, modelAccessor);
            foreach (var keyValuePair in prototype.AdditionalValues)
            {
                metadataFromPrototype.AdditionalValues.Add(keyValuePair.Key, keyValuePair.Value);
            }

            // we need to preserve DisplayName fetched during prototype creation
            metadataFromPrototype.DisplayName = prototype.DisplayName;

            return metadataFromPrototype;
        }

        protected override CachedDataAnnotationsModelMetadata CreateMetadataPrototype(IEnumerable<Attribute> attributes, Type containerType, Type modelType, string propertyName)
        {
            var theAttributes = attributes.ToList();
            var prototype = base.CreateMetadataPrototype(theAttributes, containerType, modelType, propertyName);

            try
            {
                foreach (var validationAttribute in theAttributes.OfType<ValidationAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.ErrorMessage)))
                {
                    prototype.AdditionalValues.Add(validationAttribute.GetHashCode().ToString(CultureInfo.InvariantCulture), validationAttribute.ErrorMessage);
                }
            }
            catch (Exception)
            {
                // there is weird cases when item has been added to the Dictionary already..
                // TODO: need to investigate more about this
            }

            // handle also case when [Display] attribute is not present
            if(containerType?.GetCustomAttribute<LocalizedModelAttribute>() != null)
            {
                var translation = ModelMetadataLocalizationHelper.GetValue(containerType, propertyName);
                prototype.DisplayName = translation;

                foreach (var displayAttribute in theAttributes.OfType<DisplayAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.Name)))
                {
                    displayAttribute.Name = translation;
                }
            }

            return prototype;
        }
    }
}
