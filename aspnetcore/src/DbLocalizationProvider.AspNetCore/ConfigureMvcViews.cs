// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore;

public class ConfigureMvcViews : IConfigureOptions<MvcViewOptions>
{
    private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

    public ConfigureMvcViews(IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
    {
        _validationAttributeAdapterProvider = validationAttributeAdapterProvider;
    }

    public void Configure(MvcViewOptions options)
    {
        //options.ClientModelValidatorProviders.Insert(0, new LocalizedClientModelValidator(_validationAttributeAdapterProvider));
    }
}
