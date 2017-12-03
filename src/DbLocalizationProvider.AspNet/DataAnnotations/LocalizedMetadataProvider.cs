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
                return data;

            if(containerType.GetCustomAttribute<LocalizedModelAttribute>() == null)
                return data;

            data.DisplayName = !ModelMetadataLocalizationHelper.UseLegacyMode(data.DisplayName)
                ? ModelMetadataLocalizationHelper.GetTranslation(containerType, propertyName)
                : ModelMetadataLocalizationHelper.GetTranslation(data.DisplayName);


            // TODO: extract this as decorator
            if(data.IsRequired
               && ConfigurationContext.Current.ModelMetadataProviders.MarkRequiredFields
               && ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldResource != null)
                data.DisplayName += LocalizationProvider.Current.GetStringByCulture(
                    ConfigurationContext.Current.ModelMetadataProviders.RequiredFieldResource, CultureInfo.CurrentUICulture);

            var displayAttribute = theAttributes.OfType<DisplayAttribute>().FirstOrDefault();
            if(displayAttribute?.Description != null)
                data.Description =
                    ModelMetadataLocalizationHelper.GetTranslation(containerType, $"{propertyName}-Description");

            return data;
        }
    }
}
