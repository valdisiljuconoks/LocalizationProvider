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
}
