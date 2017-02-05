using System.Collections.Generic;

namespace DbLocalizationProvider.Import
{
    public class DetectedImportChange
    {
        public DetectedImportChange() { }

        public DetectedImportChange(ChangeType changeType, LocalizationResource importing, LocalizationResource existing)
        {
            ChangeType = changeType;
            ImportingResource = importing;
            ExistingResource = existing;
        }

        public ChangeType ChangeType { get; private set; }

        public bool Selected { get; set; }

        public LocalizationResource ImportingResource { get; set; }

        public LocalizationResource ExistingResource { get; set; }

        public ICollection<string> ChangedLanguages { get; set; }
    }
}
