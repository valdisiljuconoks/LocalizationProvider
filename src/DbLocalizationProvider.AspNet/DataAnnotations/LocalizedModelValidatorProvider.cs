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
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using DbLocalizationProvider.Internal;

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
                var resourceKey = ResourceKeyBuilder.BuildResourceKey(metadata.ContainerType, metadata.PropertyName, attribute);
                var translation = ModelMetadataLocalizationHelper.GetTranslation(resourceKey);
                if(!string.IsNullOrEmpty(translation))
                {
                    attribute.ErrorMessage = translation;
                }
            }

            return base.GetValidators(metadata, context, attributes);
        }
    }
}
