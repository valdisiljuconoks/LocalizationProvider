using System;
using System.Collections.Generic;
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
            var data = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            if(containerType == null)
            {
                return data;
            }

            if(containerType.GetCustomAttribute<LocalizedModelAttribute>() != null)
            {
                data.DisplayName = ModelMetadataLocalizationHelper.GetValue(containerType, propertyName);
            }

            return data;
        }
    }
}
