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

            Resources.ForEach(r =>
                              {
                                  var trimmed = new string(r.Key.Take(UiConfigurationContext.Current.MaxResourceKeyDisplayLength).ToArray());
                                  r.DisplayKey = r.Key.Length <= UiConfigurationContext.Current.MaxResourceKeyDisplayLength ? trimmed : $"{trimmed}...";
                              });
        }

        public List<ResourceListItem> Resources { get; }

        public IEnumerable<CultureInfo> Languages { get; }

        public IEnumerable<CultureInfo> SelectedLanguages { get; }

        public bool ShowMenu { get; set; }

        public bool AdminMode { get; set; }

        public IEnumerable<ResourceTreeItem> Tree { get; set; }

        public bool IsTreeView { get; set; }
    }
}
