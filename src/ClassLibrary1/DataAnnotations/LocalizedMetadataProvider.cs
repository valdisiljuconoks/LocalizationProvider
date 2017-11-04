using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

            if(containerType.GetCustomAttribute<LocalizedModelAttribute>() == null)
                return data;

            data.DisplayName = !ModelMetadataLocalizationHelper.UseLegacyMode(data.DisplayName) ? 
                ModelMetadataLocalizationHelper.GetTranslation(containerType, propertyName)
                : ModelMetadataLocalizationHelper.GetTranslation(data.DisplayName);


            // TODO: extract this as decorator
            if(data.IsRequired
               && ConfigurationContext.Current.ModelMetadataProviders.MarkRequiredFields
               && ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldResource != null)
            {
                data.DisplayName += LocalizationProvider.Current.GetStringByCulture(ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldResource,
                                                                                    CultureInfo.CurrentUICulture);
            }

            var displayAttribute = theAttributes.OfType<DisplayAttribute>().FirstOrDefault();
            if(displayAttribute?.Description != null)
            {
                data.Description = ModelMetadataLocalizationHelper.GetTranslation(containerType, $"{propertyName}-Description");
            }

            return data;
        }
    }
}
