// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;

namespace DbLocalizationProvider.Import
{
    /// <summary>
    /// Class describing detected import change
    /// </summary>
    public class DetectedImportChange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetectedImportChange" /> class.
        /// </summary>
        public DetectedImportChange() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectedImportChange" /> class.
        /// </summary>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="importing">The importing.</param>
        /// <param name="existing">The existing.</param>
        public DetectedImportChange(ChangeType changeType, LocalizationResource importing, LocalizationResource existing)
        {
            ChangeType = changeType;
            ImportingResource = importing;
            ExistingResource = existing;
            ChangedLanguages = new List<string>();
        }

        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        public ChangeType ChangeType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DetectedImportChange" /> is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the importing resource.
        /// </summary>
        public LocalizationResource ImportingResource { get; set; }

        /// <summary>
        /// Gets or sets the existing resource.
        /// </summary>
        public LocalizationResource ExistingResource { get; set; }

        /// <summary>
        /// Gets or sets list of changed languages.
        /// </summary>
        public ICollection<string> ChangedLanguages { get; set; }
    }
}
