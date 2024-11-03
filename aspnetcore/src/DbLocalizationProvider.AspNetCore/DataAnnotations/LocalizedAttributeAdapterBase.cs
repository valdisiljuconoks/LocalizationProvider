// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public abstract class LocalizedAttributeAdapterBase<T> : AttributeAdapterBase<T> where T : ValidationAttribute
{
    private readonly IStringLocalizer _stringLocalizer;

    /// <summary>
    /// Resource key builder.
    /// </summary>
    protected ResourceKeyBuilder _resourceKeyBuilder;

    /// <inheritdoc />
    protected LocalizedAttributeAdapterBase(
        T attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        _resourceKeyBuilder = resourceKeyBuilder;
    }

    /// <inheritdoc />
    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        ArgumentNullException.ThrowIfNull(nameof(validationContext));

        return GetErrorMessage(validationContext, null);
    }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    /// <param name="validationContext">The context to use in message creation.</param>
    /// <param name="arguments">Arguments to pass to error message formatting procedure.</param>
    /// <returns>The localized error message.</returns>
    public string GetErrorMessage(ModelValidationContextBase validationContext, params object[] arguments)
    {
        ArgumentNullException.ThrowIfNull(nameof(validationContext));

        if (validationContext.ModelMetadata.ContainerType == null || validationContext.ModelMetadata.PropertyName == null)
        {
            return string.Empty;
        }

        return _stringLocalizer?.GetString(
            _resourceKeyBuilder.BuildResourceKey(
                validationContext.ModelMetadata.ContainerType,
                validationContext.ModelMetadata.PropertyName,
                Attribute),
            arguments);
    }
}
