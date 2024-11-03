// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedCompareAttributeAdapter : LocalizedAttributeAdapterBase<CompareAttribute>
{
    private readonly string _otherProperty;

    /// <inheritdoc />
    public LocalizedCompareAttributeAdapter(
        CompareAttribute attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder)
    {
        _otherProperty = "*." + attribute.OtherProperty;
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes,
                       "data-val-equalto",
                       GetErrorMessage(context, GetOtherPropertyDisplayName(context, Attribute)));
        MergeAttribute(context.Attributes, "data-val-equalto-other", _otherProperty);
    }

    private static string GetOtherPropertyDisplayName(
        ModelValidationContextBase validationContext,
        CompareAttribute attribute)
    {
        // The System.ComponentModel.DataAnnotations.CompareAttribute doesn't populate the
        // OtherPropertyDisplayName until after IsValid() is called. Therefore, at the time we get
        // the error message for client validation, the display name is not populated and won't be used.
        var otherPropertyDisplayName = attribute.OtherPropertyDisplayName;
        if (otherPropertyDisplayName != null || validationContext.ModelMetadata.ContainerType == null)
        {
            return attribute.OtherProperty;
        }

        var otherProperty = validationContext.MetadataProvider.GetMetadataForProperty(
            validationContext.ModelMetadata.ContainerType,
            attribute.OtherProperty);

        return otherProperty != null ? otherProperty.GetDisplayName() : attribute.OtherProperty;
    }
}
