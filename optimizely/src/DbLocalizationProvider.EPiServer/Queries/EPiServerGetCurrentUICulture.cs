// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using EPiServer.Core;
using EPiServer.Framework.Localization;

namespace DbLocalizationProvider.EPiServer.Queries;

public class EPiServerGetCurrentUICulture
{
    public class Handler : IQueryHandler<GetCurrentUICulture.Query, CultureInfo>
    {
        private readonly ICurrentCultureAccessor _accessor;

        public Handler(ICurrentCultureAccessor accessor)
        {
            _accessor = accessor;
        }

        public CultureInfo Execute(GetCurrentUICulture.Query query) => _accessor.CurrentUICulture;
    }
}
