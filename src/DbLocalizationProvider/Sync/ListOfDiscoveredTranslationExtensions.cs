// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Sync
{
    public static class ListOfDiscoveredTranslationExtensions
    {
        public static string DefaultTranslation(this ICollection<DiscoveredTranslation> target)
        {
            return target.FirstOrDefault(t => !string.IsNullOrEmpty(t.Culture))?.Translation;
        }
    }
}
