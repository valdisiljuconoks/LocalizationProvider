using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Export
{
    public class JsonResourceExporter : IResourceExporter
    {
        internal static JsonSerializerSettings DefaultSettings
        {
            get
            {
                var settings = new JsonSerializerSettings
                               {
                                   ContractResolver = new JsonDefaultContractResolver(),
                                   Formatting = Formatting.Indented,
                                   DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                                   Culture = CultureInfo.InvariantCulture,
                                   DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
                               };

                return settings;
            }
        }

        public ExportResult Export(ICollection<LocalizationResource> resources, NameValueCollection parameters = null)
        {
            return new ExportResult(JsonConvert.SerializeObject(resources, DefaultSettings), "application/json", $"localization-resources-{DateTime.UtcNow:yyyyMMdd}.json");
        }

        public T Deserialize<T>(string stringValue) where T : class
        {
            return JsonConvert.DeserializeObject<T>(stringValue, DefaultSettings);
        }

        public string FormatName => "JSON";
        public string ProviderId => "json";
    }
}
