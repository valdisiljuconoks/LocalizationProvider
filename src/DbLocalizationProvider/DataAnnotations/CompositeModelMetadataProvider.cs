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
