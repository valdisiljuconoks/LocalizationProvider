// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetAllTranslations
    {
        public class Query : IQuery<IEnumerable<ResourceItem>>
        {
            public Query(string key, CultureInfo language)
            {
                Key = key;
                Language = language;
            }

            public string Key { get; set; }

            public CultureInfo Language { get; set; }
        }
    }
}
