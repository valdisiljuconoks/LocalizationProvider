using System.Globalization;

namespace DbLocalizationProvider.AdminUI
{
    public class ResourceItem
    {
        public ResourceItem(string key, string value, CultureInfo sourceCulture)
        {
            Key = key;
            Value = value;
            SourceCulture = sourceCulture;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public CultureInfo SourceCulture { get; set; }
    }
}
