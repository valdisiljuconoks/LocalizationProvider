// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <inheritdoc />
public class LocalizedAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _base = new ValidationAttributeAdapterProvider();
    private readonly ResourceKeyBuilder _keyBuilder;

    /// <summary>
    /// Creates new instance of <c>LocalizedAttributeAdapterProvider</c>.
    /// </summary>
    /// <param name="keyBuilder">Resource key builder (will be required to properly form resource key).</param>
    public LocalizedAttributeAdapterProvider(ResourceKeyBuilder keyBuilder)
    {
        _keyBuilder = keyBuilder;
    }

    /// <inheritdoc />
    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    {
        return attribute switch
        {
            CompareAttribute compareAttribute =>
                new LocalizedCompareAttributeAdapter(compareAttribute, stringLocalizer, _keyBuilder),
            CreditCardAttribute creditCardAttribute =>
                new LocalizedDataTypeAttributeAdapter(creditCardAttribute, "data-val-creditcard", stringLocalizer, _keyBuilder),
            EmailAddressAttribute emailAddressAttribute =>
                new LocalizedDataTypeAttributeAdapter(emailAddressAttribute, "data-val-email", stringLocalizer, _keyBuilder),
            FileExtensionsAttribute fileExtensionsAttribute =>
                new LocalizedFileExtensionsAttributeAdapter(fileExtensionsAttribute, stringLocalizer, _keyBuilder),
            PhoneAttribute phoneAttribute =>
                new LocalizedDataTypeAttributeAdapter(phoneAttribute, "data-val-phone", stringLocalizer, _keyBuilder),
            UrlAttribute urlAttribute =>
                new LocalizedDataTypeAttributeAdapter(urlAttribute, "data-val-url", stringLocalizer, _keyBuilder),
            MaxLengthAttribute maxLengthAttribute =>
                new LocalizedMaxLengthAttributeAdapter(maxLengthAttribute, stringLocalizer, _keyBuilder),
            MinLengthAttribute minLengthAttribute =>
                new LocalizedMinLengthAttributeAdapter(minLengthAttribute, stringLocalizer, _keyBuilder),
            RangeAttribute rangeAttribute =>
                new LocalizedRangeAttributeAdapter(rangeAttribute, stringLocalizer, _keyBuilder),
            RegularExpressionAttribute regularExpressionAttribute =>
                new LocalizedRegularExpressionAttributeAdapter(regularExpressionAttribute, stringLocalizer, _keyBuilder),
            RequiredAttribute requiredAttribute =>
                new LocalizedRequiredAttributeAdapter(requiredAttribute, stringLocalizer, _keyBuilder),
            StringLengthAttribute stringLengthAttribute =>
                new LocalizedStringLengthAttributeAdapter(stringLengthAttribute, stringLocalizer, _keyBuilder),
            _ => _base.GetAttributeAdapter(attribute, stringLocalizer)
        };
    }
}
