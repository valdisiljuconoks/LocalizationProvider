// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedDataTypeAttributeAdapter : LocalizedAttributeAdapterBase<DataTypeAttribute>
{
    /// <inheritdoc />
    public LocalizedDataTypeAttributeAdapter(
        DataTypeAttribute attribute,
        string ruleName,
        IStringLocalizer stringLocalizer,
        ResourceKeyBuilder resourceKeyBuilder) : base(attribute, stringLocalizer, resourceKeyBuilder)
    {
        if (string.IsNullOrEmpty(ruleName))
        {
            throw new ArgumentNullException(nameof(ruleName));
        }

        RuleName = ruleName;
    }

    /// <summary>
    /// Name of the rule to check.
    /// </summary>
    public string RuleName { get; set; }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, RuleName, GetErrorMessage(context, Attribute.GetDataTypeName()));
    }
}
