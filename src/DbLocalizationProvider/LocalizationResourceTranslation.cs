// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using Newtonsoft.Json;

namespace DbLocalizationProvider
{
    /// <summary>
    /// Represents translation of the resource if given language.
    /// </summary>
    public class LocalizationResourceTranslation
    {
        private string _language;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier to which translation belongs to.
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the localization resource.
        /// </summary>
        [JsonIgnore]
        public LocalizationResource LocalizationResource { get; set; }

        /// <summary>
        /// Gets or sets the language for the translation.
        /// </summary>
        public string Language
        {
            get => _language;
            set
            {
                var c = new CultureInfo(value);
                _language = c.Name;
            }
        }

        /// <summary>
        /// Gets or sets actual translation value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime ModificationDate { get; set; }
    }
}
