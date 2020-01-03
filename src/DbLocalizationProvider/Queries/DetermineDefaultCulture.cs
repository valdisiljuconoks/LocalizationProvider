// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class DetermineDefaultCulture
    {
        public class Query : IQuery<string> { }

        public class Handler : IQueryHandler<Query, string>
        {
            public string Execute(Query query)
            {
                return ConfigurationContext.Current.DefaultResourceCulture != null
                           ? ConfigurationContext.Current.DefaultResourceCulture.Name
                           : "en";
            }
        }
    }
}
