// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetAllResources
    {
        public class Query : IQuery<IEnumerable<LocalizationResource>>
        {
            public Query(bool forceReadFromDb = false)
            {
                ForceReadFromDb = forceReadFromDb;
            }

            public bool ForceReadFromDb { get; }
        }
    }
}
