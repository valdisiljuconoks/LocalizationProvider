using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace DbLocalizationProvider.DataAnnotations
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
            var theAttributes = attributes.ToList();
            var data = base.CreateMetadata(theAttributes, containerType, modelAccessor, modelType, propertyName);

            if(containerType == null)
            {
                return data;
            }

            if(containerType.GetCustomAttribute<LocalizedModelAttribute>() != null)
            {
                data.DisplayName = ModelMetadataLocalizationHelper.GetValue(containerType, propertyName);

                if(ConfigurationContext.Current.ModelMetadataProviders.MarkRequiredFields)
                {
                    data.DisplayName += ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldIndicator;
                }

                var displayAttribute = theAttributes.OfType<DisplayAttribute>().FirstOrDefault();

                if(!string.IsNullOrEmpty(displayAttribute?.Description))
                {
                    data.Description = ModelMetadataLocalizationHelper.GetValue(containerType, $"{propertyName}-Description");
                }
            }

            return data;
        }
    }
}
