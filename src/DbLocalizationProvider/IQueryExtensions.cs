// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using DbLocalizationProvider.Abstractions;

namespace DbLocalizationProvider
{
    public static class IQueryExtensions
    {
        public static TResult Execute<TResult>(this IQuery<TResult> query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var handler = ConfigurationContext.Current.TypeFactory.GetQueryHandler(query);

            return handler.Execute(query);
        }
    }
}
