using System.Globalization;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Export
{
    public class JsonDataSerializer 
    {
        public static JsonSerializerSettings DefaultSettings
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

        public string Serialize<T>(T objectValue) where T : class
        {
            return JsonConvert.SerializeObject(objectValue, DefaultSettings);
        }

        public T Deserialize<T>(string stringValue) where T : class
        {
            return JsonConvert.DeserializeObject<T>(stringValue, DefaultSettings);
        }
    }
}
