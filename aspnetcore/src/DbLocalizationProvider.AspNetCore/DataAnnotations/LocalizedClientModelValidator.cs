// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <summary>
/// Provides a collection of <see cref="IClientModelValidator" />s.
/// </summary>
public class LocalizedClientModelValidator : IClientModelValidatorProvider
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly ExpressionHelper _expressionHelper;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly ILocalizationProvider _localizationProvider;
    private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

    /// <summary>
    /// Creates new instance of localized client-side validator.
    /// </summary>
    /// <param name="validationAttributeAdapterProvider"></param>
    /// <param name="localizationProvider"></param>
    /// <param name="keyBuilder"></param>
    /// <param name="expressionHelper"></param>
    /// <param name="configurationContext"></param>
    public LocalizedClientModelValidator(
        IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
        ILocalizationProvider localizationProvider,
        ResourceKeyBuilder keyBuilder,
        ExpressionHelper expressionHelper,
        IOptions<ConfigurationContext> configurationContext)
    {
        _validationAttributeAdapterProvider = validationAttributeAdapterProvider;
        _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        _keyBuilder = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
        _expressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
        _configurationContext = configurationContext ?? throw new ArgumentNullException(nameof(configurationContext));
    }

    /// <summary>
    /// Creates set of <see cref="IClientModelValidator" />s by updating
    /// <see cref="ClientValidatorItem.Validator" /> in <see cref="ClientValidatorProviderContext.Results" />.
    /// </summary>
    /// <param name="context">The <see cref="ClientModelValidationContext" /> associated with this call.</param>
    public void CreateValidators(ClientValidatorProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var type = context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType;
        var isReusable = _configurationContext.Value.ModelMetadataProviders.UseCachedProviders;
        var flag = false;

        foreach (var result in context.Results)
        {
            if (result.Validator != null)
            {
                flag |= result.Validator is RequiredAttributeAdapter;
            }
            else
            {
                if (result.ValidatorMetadata is ValidationAttribute validatorMetadata)
                {
                    flag |= validatorMetadata is RequiredAttribute;
                    var attributeAdapter = _validationAttributeAdapterProvider.GetAttributeAdapter(validatorMetadata,
                        new ValidationStringLocalizer(type,
                                                      context.ModelMetadata.PropertyName,
                                                      validatorMetadata,
                                                      _localizationProvider,
                                                      _keyBuilder,
                                                      _expressionHelper));

                    if (attributeAdapter != null)
                    {
                        result.Validator = attributeAdapter;
                        result.IsReusable = isReusable;
                    }
                }
            }
        }

        if (flag || !context.ModelMetadata.IsRequired)
        {
            return;
        }

        context.Results.Add(new ClientValidatorItem
        {
            Validator = _validationAttributeAdapterProvider.GetAttributeAdapter(
                new RequiredAttribute(),
                new ValidationStringLocalizer(type,
                                              context.ModelMetadata.PropertyName,
                                              new RequiredAttribute(),
                                              _localizationProvider,
                                              _keyBuilder,
                                              _expressionHelper)),
            IsReusable = isReusable
        });
    }
}
