// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedRegularExpressionAttributeAdapter : LocalizedAttributeAdapterBase<RegularExpressionAttribute>
{
    /// <inheritdoc />
    public LocalizedRegularExpressionAttributeAdapter(
        RegularExpressionAttribute attribute,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder) { }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-regex", GetErrorMessage(context, Attribute.Pattern));
        MergeAttribute(context.Attributes, "data-val-regex-pattern", Attribute.Pattern);
    }
}
