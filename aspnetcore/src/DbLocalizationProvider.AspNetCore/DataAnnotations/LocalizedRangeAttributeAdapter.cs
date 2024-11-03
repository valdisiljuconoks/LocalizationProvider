// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedRangeAttributeAdapter : LocalizedAttributeAdapterBase<RangeAttribute>
{
    private readonly string _max;
    private readonly string _min;

    /// <inheritdoc />
    public LocalizedRangeAttributeAdapter(
        RangeAttribute attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder)
    {
        // This will trigger the conversion of Attribute.Minimum and Attribute.Maximum.
        // This is needed, because the attribute is stateful and will convert from a string like
        // "100m" to the decimal value 100.
        //
        // Validate a randomly selected number.
        attribute.IsValid(3);

        _max = Convert.ToString(Attribute.Maximum, CultureInfo.InvariantCulture);
        _min = Convert.ToString(Attribute.Minimum, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-range", GetErrorMessage(context, Attribute.Minimum, Attribute.Maximum));
        MergeAttribute(context.Attributes, "data-val-range-max", _max);
        MergeAttribute(context.Attributes, "data-val-range-min", _min);
    }
}
