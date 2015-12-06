using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Localization;

namespace TechFellow.DbLocalizationProvider.AdminUI
{
    public class LocalizationResourceViewModel
    {
        public LocalizationResourceViewModel(List<KeyValuePair<string, List<ResourceItem>>> resources, IEnumerable<CultureInfo> languages)
        {
            Resources = resources;
            Languages = languages;
        }

        public List<KeyValuePair<string, List<ResourceItem>>> Resources { get; }

        public IEnumerable<CultureInfo> Languages { get; }
    }
}
