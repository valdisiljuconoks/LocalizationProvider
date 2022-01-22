// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.AdminUI.Models
{
    /// <summary>
    /// Grand parent for the rest of the family
    /// </summary>
    public abstract class BaseApiModel
    {
        /// <summary>
        /// Creates new instance of parent
        /// </summary>
        /// <param name="languages">List of supported languages</param>
        /// <param name="visibleLanguages">List of visible languages</param>
        public BaseApiModel(IEnumerable<AvailableLanguage> languages, IEnumerable<AvailableLanguage> visibleLanguages)
        {
            Languages = languages
                .OrderBy(a => a.SortIndex)
                .Select(l => new CultureApiModel(l.CultureInfo.Name, l.DisplayName));

            VisibleLanguages = visibleLanguages
                .OrderBy(a => a.SortIndex)
                .Select(l => new CultureApiModel(l.CultureInfo.Name, l.DisplayName));
        }

        /// <summary>
        /// List of localized resources. They are represented as `JObject` here is requires dynamic structure.
        /// </summary>
        public List<JObject> Resources { get; protected set; }

        /// <summary>
        /// List of supported languages
        /// </summary>
        public IEnumerable<CultureApiModel> Languages { get; protected set; }

        /// <summary>
        /// List of supported languages
        /// </summary>
        public IEnumerable<CultureApiModel> VisibleLanguages { get; protected set; }

        /// <summary>
        /// What kind of options AdminUI should take into account while returning result
        /// </summary>
        public UiOptions Options { get; protected set; } = new UiOptions();
    }
}
