// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class DataAnnotationLocalizerProviderSetup : IConfigureOptions<MvcDataAnnotationsLocalizationOptions>
{
    /// <inheritdoc />
    public void Configure(MvcDataAnnotationsLocalizationOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.DataAnnotationLocalizerProvider = (modelType, stringLocalizerFactory) => stringLocalizerFactory.Create(modelType);
    }
}
