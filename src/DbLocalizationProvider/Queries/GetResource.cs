// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetResource
    {
        public class Query : IQuery<LocalizationResource>
        {
            public Query(string resourceKey)
            {
                ResourceKey = resourceKey ?? throw new ArgumentNullException(nameof(resourceKey));
            }

            public string ResourceKey { get; }
        }
    }
}
