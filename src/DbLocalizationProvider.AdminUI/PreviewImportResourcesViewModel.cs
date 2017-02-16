using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Import;

namespace DbLocalizationProvider.AdminUI
{
    public class PreviewImportResourcesViewModel
    {
        public PreviewImportResourcesViewModel(IEnumerable<DetectedImportChange> changes, bool showMenu, IEnumerable<CultureInfo> languages)
        {
            Changes = changes;
            ShowMenu = showMenu;
            Languages = languages;
        }

        public IEnumerable<DetectedImportChange> Changes { get; private set; }

        public bool ShowMenu { get; private set; }

        public IEnumerable<CultureInfo> Languages { get; private set; }
    }
}
