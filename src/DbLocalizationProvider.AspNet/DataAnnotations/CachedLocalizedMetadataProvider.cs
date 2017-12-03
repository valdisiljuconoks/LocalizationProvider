// Copyright © 2017 Valdis Iljuconoks.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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

            foreach (var validationAttribute in theAttributes.OfType<ValidationAttribute>().Where(a => !string.IsNullOrWhiteSpace(a.ErrorMessage)))
            {
                try
                {
                    prototype.AdditionalValues.Add(validationAttribute.GetHashCode().ToString(CultureInfo.InvariantCulture), validationAttribute.ErrorMessage);
                }
                catch (Exception)
                {
                    // there is weird cases when item has been added to the Dictionary already..
                    // TODO: need to investigate more about this
                }
            }

            // handle also case when [Display] attribute is not present
            if(containerType?.GetCustomAttribute<LocalizedModelAttribute>() == null)
                return prototype;

            var translation = ModelMetadataLocalizationHelper.GetTranslation(containerType, propertyName);
            prototype.DisplayName = translation;

            if(prototype.IsRequired
               && ConfigurationContext.Current.ModelMetadataProviders.MarkRequiredFields
               && ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldResource != null)
            {
                prototype.DisplayName += LocalizationProvider.Current.GetStringByCulture(ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldResource,
                                                                                         CultureInfo.CurrentUICulture);
            }

            var displayAttribute = theAttributes.OfType<DisplayAttribute>().FirstOrDefault();
            if(!string.IsNullOrEmpty(displayAttribute?.Name))
            {
                displayAttribute.Name = translation;
            }

            if(!string.IsNullOrEmpty(displayAttribute?.Description))
            {
                prototype.Description = ModelMetadataLocalizationHelper.GetTranslation(containerType, $"{propertyName}-Description");
            }

            return prototype;
        }
    }
}
