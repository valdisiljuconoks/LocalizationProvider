// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedMinLengthAttributeAdapter : LocalizedAttributeAdapterBase<MinLengthAttribute>
{
    private readonly string _min;

    /// <inheritdoc />
    public LocalizedMinLengthAttributeAdapter(
        MinLengthAttribute attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder)
    {
        _min = Attribute.Length.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-minlength", GetErrorMessage(context, Attribute.Length));
        MergeAttribute(context.Attributes, "data-val-minlength-min", _min);
    }
}
