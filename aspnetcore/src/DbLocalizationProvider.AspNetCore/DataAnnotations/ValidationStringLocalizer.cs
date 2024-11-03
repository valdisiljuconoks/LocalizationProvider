// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Internal;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

public class ValidationStringLocalizer : IStringLocalizer, ILocalizationServicesAccessor, ICultureAwareStringLocalizer
{
    private readonly Type _containerType;
    private readonly CultureInfo _culture;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly ILocalizationProvider _localizationProvider;
    private readonly string _propertyName;
    private readonly ValidationAttribute _validatorMetadata;

    public ValidationStringLocalizer(
        Type containerType,
        string propertyName,
        ValidationAttribute validatorMetadata,
        ILocalizationProvider localizationProvider,
        ResourceKeyBuilder keyBuilder,
        ExpressionHelper expressionHelper)
        : this(containerType,
               propertyName,
               validatorMetadata,
               CultureInfo.CurrentUICulture,
               localizationProvider,
               keyBuilder,
               expressionHelper) { }

    private ValidationStringLocalizer(
        Type containerType,
        string propertyName,
        ValidationAttribute validatorMetadata,
        CultureInfo culture,
        ILocalizationProvider localizationProvider,
        ResourceKeyBuilder keyBuilder,
        ExpressionHelper expressionHelper)
    {
        _containerType = containerType;
        _propertyName = propertyName;
        _validatorMetadata = validatorMetadata;
        _culture = culture;
        _localizationProvider = localizationProvider;
        _keyBuilder = keyBuilder;
        ExpressionHelper = expressionHelper;
    }

    public IStringLocalizer ChangeLanguage(CultureInfo language)
    {
        return new ValidationStringLocalizer(_containerType,
                                             _propertyName,
                                             _validatorMetadata,
                                             language,
                                             _localizationProvider,
                                             _keyBuilder,
                                             ExpressionHelper);
    }

    public ExpressionHelper ExpressionHelper { get; }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return Enumerable.Empty<LocalizedString>();
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = _localizationProvider.GetString(
                _keyBuilder.BuildResourceKey(_containerType, _propertyName, _validatorMetadata));

            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = _localizationProvider.GetStringByCulture(
                _keyBuilder.BuildResourceKey(_containerType, _propertyName, _validatorMetadata),
                _culture,
                arguments);

            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        return ChangeLanguage(culture);
    }
}
