// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;

public class AdminUIMarkerMiddleware
{
    private readonly RequestDelegate _next;

    public AdminUIMarkerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        return _next(context);
    }
}
