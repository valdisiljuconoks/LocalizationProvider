using System;
using Newtonsoft.Json.Serialization;

namespace DbLocalizationProvider.Export
{
    public class JsonDefaultContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
        {
            var contract = base.CreateDictionaryContract(objectType);
            contract.PropertyNameResolver = propertyName => propertyName;

            return contract;
        }
    }
}
