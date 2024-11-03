// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;

public class CheckRoutingMiddleware
{
    private static readonly ConcurrentDictionary<string, object> _middlewareNames = new();
    private static readonly string markerMiddlewareName = typeof(AdminUIMarkerMiddleware).FullName;
    private readonly RequestDelegate _next;

    public CheckRoutingMiddleware(RequestDelegate next)
    {
        _next = next;
        var name = next.Target.GetType().FullName;
        _middlewareNames.TryAdd(name, null);

        if (name != markerMiddlewareName)
        {
            return;
        }

        // if `AdminUIMarkerMiddleware` middleware is registered then
        // AdminUI has been added - let's check if we have routing (mvc or endpoint) in place already
        if (_middlewareNames.ContainsKey("Microsoft.AspNetCore.Builder.RouterMiddleware")
            || _middlewareNames.ContainsKey("Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware"))
        {
            throw new InvalidOperationException(
                "Routing has been already initialized. Invoke 'UseDbLocalizationProviderAdminUI' before routing system setup.");
        }
    }

    public Task Invoke(HttpContext context)
    {
        return _next(context);
    }
}
