// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedStringLengthAttributeAdapter : LocalizedAttributeAdapterBase<StringLengthAttribute>
{
    private readonly string _max;
    private readonly string _min;

    /// <inheritdoc />
    public LocalizedStringLengthAttributeAdapter(
        StringLengthAttribute attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder)
    {
        _max = Attribute.MaximumLength.ToString(CultureInfo.InvariantCulture);
        _min = Attribute.MinimumLength.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-length", GetErrorMessage(context, _min, _max));

        if (Attribute.MaximumLength != int.MaxValue)
        {
            MergeAttribute(context.Attributes, "data-val-length-max", _max);
        }

        if (Attribute.MinimumLength != 0)
        {
            MergeAttribute(context.Attributes, "data-val-length-min", _min);
        }
    }
}
