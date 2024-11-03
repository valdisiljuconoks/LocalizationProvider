// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Http;

namespace DbLocalizationProvider.AspNetCore.ClientsideProvider;

internal class AppContext
{
    public static IHttpContextAccessor Service { get; private set; }

    public static void Configure(IHttpContextAccessor service)
    {
        Service = service;
    }
}
