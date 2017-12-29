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
using System.Web.Mvc;

namespace DbLocalizationProvider.DataAnnotations
{
    public class CompositeModelMetadataProvider<TProvider> : ModelMetadataProvider where TProvider : ModelMetadataProvider, new()
    {
        private readonly ModelMetadataProvider _innerProvider;
        private readonly TProvider _wrappedProvider;

        public CompositeModelMetadataProvider(ModelMetadataProvider innerProvider)
        {
            _innerProvider = innerProvider;

            // TODO: pass this is via .ctor
            _wrappedProvider = new TProvider();
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            return _wrappedProvider.GetMetadataForProperties(container, containerType);
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var metadata = _wrappedProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName);

            if(_innerProvider == null)
            {
                return metadata;
            }

            var additionalMetadata = _innerProvider.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            MergeAdditionalValues(metadata.AdditionalValues, additionalMetadata.AdditionalValues);

            return metadata;
        }

        private void MergeAdditionalValues(IDictionary<string, object> target, Dictionary<string, object> source)
        {
            foreach (var key in source.Keys)
            {
                if(!target.ContainsKey(key))
                {
                    target.Add(key, source[key]);
                }
            }
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            return _wrappedProvider.GetMetadataForType(modelAccessor, modelType);
        }
    }
}
