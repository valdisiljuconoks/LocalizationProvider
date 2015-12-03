using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Localization;

namespace TechFellow.DbLocalizationProvider
{
    public class LocalizationResourceViewModel
    {
        public LocalizationResourceViewModel(List<KeyValuePair<string, List<ResourceItem>>> allResources, IEnumerable<CultureInfo> languages)
        {
            AllResources = allResources;
            Languages = languages;
        }

        public List<KeyValuePair<string, List<ResourceItem>>> AllResources { get; }

        public IEnumerable<CultureInfo> Languages { get; }
    }
}
