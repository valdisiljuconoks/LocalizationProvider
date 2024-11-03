// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;

public class AdminUIVerificationApplicationBuilder : IApplicationBuilder
{
    private readonly IApplicationBuilder _inner;

    public AdminUIVerificationApplicationBuilder(IApplicationBuilder inner)
    {
        _inner = inner;
    }

    public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
    {
        return _inner
            .UseMiddleware<CheckRoutingMiddleware>()
            .Use(middleware);
    }

    public IApplicationBuilder New()
    {
        return new AdminUIVerificationApplicationBuilder(_inner);
    }

    public RequestDelegate Build()
    {
        return _inner.Build();
    }

    public IServiceProvider ApplicationServices
    {
        get => _inner.ApplicationServices;
        set => _inner.ApplicationServices = value;
    }

    public IFeatureCollection ServerFeatures => _inner.ServerFeatures;

    public IDictionary<string, object> Properties => _inner.Properties;
}
