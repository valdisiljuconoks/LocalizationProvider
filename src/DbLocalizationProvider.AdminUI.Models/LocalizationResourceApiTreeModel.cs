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
    /// View model that contains data that could help you to generate resource list in tree mode
    /// </summary>
    public class LocalizationResourceApiTreeModel : BaseApiModel
    {
        private const string _segmentPropertyName = "segmentKey";
        private readonly int _listDisplayLength;
        private readonly int _popupTitleLength;
        private readonly char[] _splitChars;

        /// <summary>
        /// Create new instance of the model
        /// </summary>
        /// <param name="resources">List of localized resources</param>
        /// <param name="languages">What languages are supported</param>
        /// <param name="visibleLanguages">Which languages are visible</param>
        /// <param name="popupTitleLength">How many symbols are possible to show in the modal title bar</param>
        /// <param name="listDisplayLength">How many of resource key will be visible in the list</param>
        /// <param name="options">What kind of options should be taken into account while generating the results</param>
        /// <param name="legacyMode">If legacy mode is active, resource keys should also be split on '/'</param>
        public LocalizationResourceApiTreeModel(
            List<LocalizationResource> resources,
            IEnumerable<AvailableLanguage> languages,
            IEnumerable<AvailableLanguage> visibleLanguages,
            int popupTitleLength,
            int listDisplayLength,
            UiOptions options,
            bool legacyMode = false) : base(languages, visibleLanguages)
        {
            _popupTitleLength = popupTitleLength;
            _listDisplayLength = listDisplayLength;
            _splitChars = legacyMode ? new[] { '.', '+', '/' } : new[] { '.', '+' };

            Resources = ConvertToApiModel(resources);
            Options = options;
        }

        internal List<JObject> ConvertToApiModel(List<LocalizationResource> resources)
        {
            var result = new JArray();
            foreach (var resource in resources)
            {
                var segments = resource.ResourceKey.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries).ToList();

                void AddChildrenNodes(
                    JArray children,
                    List<string> list,
                    LocalizationResource localizationResource,
                    int currentLevel,
                    int depth)
                {
                    while (true)
                    {
                        var (head, tail) = list;
                        var el = children.FirstOrDefault(c => c[_segmentPropertyName] != null
                                                              && c[_segmentPropertyName]
                                                                  .ToString()
                                                                  .Equals(head, StringComparison.InvariantCultureIgnoreCase));
                        if (el == null)
                        {
                            el = new JObject { [_segmentPropertyName] = head };
                            if (currentLevel == depth)
                            {
                                // process leaf
                                var key = localizationResource.ResourceKey;
                                el["resourceKey"] = key;
                                el["displayKey"] =
                                    $"{key.Substring(0, key.Length > _listDisplayLength ? _listDisplayLength : key.Length)}{(key.Length > _listDisplayLength ? "..." : "")}";
                                el["titleKey"] =
                                    $"{(key.Length > _popupTitleLength ? "..." : "")}{key.Substring(key.Length - Math.Min(_popupTitleLength, key.Length))}";
                                el["syncedFromCode"] = localizationResource.FromCode;
                                el["isModified"] = localizationResource.IsModified;
                                el["allowDelete"] = !localizationResource.FromCode;
                                el["translation"] = localizationResource.Translations.FindByLanguage(CultureInfo.InvariantCulture)
                                    ?.Value;
                                foreach (var language in Languages)
                                {
                                    el["translation-" + language.Code] =
                                        localizationResource.Translations.FindByLanguage(language.Code)?.Value;
                                }
                            }
                            else
                            {
                                el["_children"] = new JArray();
                                el["_classes"] = new JObject { ["row"] = new JObject { ["parent-row"] = true } };
                            }

                            children.Add(el);
                        }

                        if (tail.Any())
                        {
                            children = el["_children"] as JArray ?? new JArray();
                            list = tail;
                            currentLevel += 1;
                            continue;
                        }

                        break;
                    }
                }

                if (segments.Any())
                {
                    AddChildrenNodes(result, segments, resource, 0, segments.Count - 1);
                }
            }

            return result.Cast<JObject>().ToList();
        }
    }
}
