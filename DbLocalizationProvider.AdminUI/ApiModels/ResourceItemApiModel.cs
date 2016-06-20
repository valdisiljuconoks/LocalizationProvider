using System.Globalization;

namespace DbLocalizationProvider.AdminUI.ApiModels
{
    public class ResourceItemApiModel
    {
        public ResourceItemApiModel(string key, string value, string sourceCulture)
        {
            Key = key;
            Value = value;

            var culture = new CultureInfo(sourceCulture);
            SourceCulture = new CultureApiModel(culture.Name, culture.EnglishName);
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public CultureApiModel SourceCulture { get; set; }
    }
}
