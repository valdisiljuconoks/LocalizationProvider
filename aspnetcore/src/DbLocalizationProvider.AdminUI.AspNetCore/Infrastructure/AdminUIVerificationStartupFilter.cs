// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace DbLocalizationProvider.AdminUI.AspNetCore.Infrastructure;

public class AdminUIVerificationStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            var wrappedBuilder = new AdminUIVerificationApplicationBuilder(builder);
            next(wrappedBuilder);
        };
    }
}
