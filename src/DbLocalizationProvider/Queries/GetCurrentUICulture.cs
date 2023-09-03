// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider.Queries
{
    public class GetCurrentUICulture
    {
        public class Query : IQuery<CultureInfo> { }

        public class Handler : IQueryHandler<Query, CultureInfo>
        {
            public Task<CultureInfo> Execute(Query query) => Task.FromResult(CultureInfo.CurrentUICulture);
        }
    }
}
