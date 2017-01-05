using System.Collections.Generic;
using DbLocalizationProvider.Import;

namespace DbLocalizationProvider.AdminUI
{
    public class PreviewImportResourcesViewModel
    {
        public PreviewImportResourcesViewModel(IEnumerable<DetectedImportChange> changes, bool showMenu)
        {
            Changes = changes;
            ShowMenu = showMenu;
        }

        public IEnumerable<DetectedImportChange> Changes { get; private set; }

        public bool ShowMenu { get; private set; }
    }
}
