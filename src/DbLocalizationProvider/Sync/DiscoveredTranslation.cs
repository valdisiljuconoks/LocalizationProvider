// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider.Sync
{
    [DebuggerDisplay("Translation: {Translation} / Culture: {Culture}")]
    public class DiscoveredTranslation
    {
        public DiscoveredTranslation(string translation, string culture)
        {
            Translation = translation;
            Culture = culture;
        }

        public string Translation { get; internal set; }

        public string Culture { get; }

        public static List<DiscoveredTranslation> FromSingle(string translation)
        {
            var defaultCulture = new DetermineDefaultCulture.Query().Execute();
            var result = new List<DiscoveredTranslation>
            {
                // invariant translation
                new DiscoveredTranslation(translation, CultureInfo.InvariantCulture.Name)
            };

            // register additional culture if default is not set to invariant
            if (defaultCulture != string.Empty)
            {
                result.Add(new DiscoveredTranslation(translation, defaultCulture));
            }

            return result;
        }
    }
}
