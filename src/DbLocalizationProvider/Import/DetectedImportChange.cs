// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

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
            ChangedLanguages = new List<string>();
        }

        public ChangeType ChangeType { get; set; }

        public bool Selected { get; set; }

        public LocalizationResource ImportingResource { get; set; }

        public LocalizationResource ExistingResource { get; set; }

        public ICollection<string> ChangedLanguages { get; set; }
    }
}
