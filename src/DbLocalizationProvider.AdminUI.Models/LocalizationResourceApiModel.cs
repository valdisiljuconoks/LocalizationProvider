// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using Newtonsoft.Json.Linq;

namespace DbLocalizationProvider.AdminUI.Models
{
    /// <summary>
    /// Return model of the AdminUI view
    /// </summary>
    public class LocalizationResourceApiModel : BaseApiModel
    {
        private readonly int _listDisplayLength;
        private readonly int _popupTitleLength;

        /// <summary>
        /// Create new instance of the model
        /// </summary>
        /// <param name="resources">List of localized resources</param>
        /// <param name="languages">What languages are supported</param>
        /// <param name="visibleLanguages">Which languages are visible</param>
        /// <param name="popupTitleLength">How many symbols are possible to show in the modal title bar</param>
        /// <param name="listDisplayLength">How many of resource key will be visible in the list</param>
        /// <param name="options">What kind of options should be taken into account while generating the results</param>
        public LocalizationResourceApiModel(
            ICollection<LocalizationResource> resources,
            IEnumerable<CultureInfo> languages,
            IEnumerable<CultureInfo> visibleLanguages,
            int popupTitleLength,
            int listDisplayLength,
            UiOptions options) : base(languages, visibleLanguages)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            if (languages == null)
            {
                throw new ArgumentNullException(nameof(languages));
            }

            _popupTitleLength = popupTitleLength;
            _listDisplayLength = listDisplayLength;
            Resources = resources.Select(r => ConvertToApiModel(r, languages)).ToList();
            Options = options;
        }

        private JObject ConvertToApiModel(LocalizationResource resource, IEnumerable<CultureInfo> languages)
        {
            var key = resource.ResourceKey;
            var result = new JObject
            {
                ["key"] = key,
                ["displayKey"] =
                    $"{key.Substring(0, key.Length > _listDisplayLength ? _listDisplayLength : key.Length)}{(key.Length > _listDisplayLength ? "..." : "")}",
                ["titleKey"] =
                    $"{(key.Length > _popupTitleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(_popupTitleLength, key.Length))}",
                ["syncedFromCode"] = resource.FromCode,
                ["isModified"] = resource.IsModified,
                ["_"] = resource.Translations.FindByLanguage(CultureInfo.InvariantCulture)?.Value,
                ["isHidden"] = resource.IsHidden ?? false,
                ["isFromCode"] = resource.FromCode
            };

            foreach (var language in languages)
            {
                result[language.Name] = resource.Translations.FindByLanguage(language)?.Value;
            }

            return result;
        }
    }
}
