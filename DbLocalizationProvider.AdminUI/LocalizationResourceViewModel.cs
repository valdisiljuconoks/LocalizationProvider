using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DbLocalizationProvider.AdminUI
{
    public class LocalizationResourceViewModel
    {
        public LocalizationResourceViewModel(List<ResourceListItem> resources, IEnumerable<CultureInfo> languages, IEnumerable<string> selectedLanguages)
        {
            Resources = resources;
            Languages = languages;
            SelectedLanguages = selectedLanguages?.Select(l => new CultureInfo(l)) ?? languages;
        }

        public List<ResourceListItem> Resources { get; }

        public IEnumerable<CultureInfo> Languages { get; }

        public IEnumerable<CultureInfo> SelectedLanguages { get; }

        public bool ShowMenu { get; set; }

        public bool AdminMode { get; set; }
    }

    public class ResourceListItem
    {
        public ResourceListItem(string key, List<ResourceItem> translations, bool allowDelete)
        {
            Key = key;
            Value = translations;
            AllowDelete = allowDelete;
        }

        public string Key { get; private set; }

        public List<ResourceItem> Value { get; private set; }

        public bool AllowDelete { get; private set; }
    }
}
