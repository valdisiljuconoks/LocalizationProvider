// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedFileExtensionsAttributeAdapter : LocalizedAttributeAdapterBase<FileExtensionsAttribute>
{
    private readonly string _extensions;
    private readonly string _formattedExtensions;

    /// <inheritdoc />
    public LocalizedFileExtensionsAttributeAdapter(
        FileExtensionsAttribute attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder)
    {
        var normalizedExtensions = Attribute.Extensions.Replace(" ", string.Empty).Replace(".", string.Empty).ToLowerInvariant();
        var parsedExtensions = normalizedExtensions.Split(',').Select(e => "." + e);
        _formattedExtensions = string.Join(", ", parsedExtensions);
        _extensions = string.Join(",", parsedExtensions);
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-fileextensions", GetErrorMessage(context, _formattedExtensions));
        MergeAttribute(context.Attributes, "data-val-fileextensions-extensions", _extensions);
    }
}
