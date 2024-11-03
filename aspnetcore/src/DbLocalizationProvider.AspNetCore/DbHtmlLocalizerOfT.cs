// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Html localizer with access to required services
/// </summary>
/// <typeparam name="TResource">Resource class to use when localizing</typeparam>
public class DbHtmlLocalizer<TResource> : HtmlLocalizer<TResource>, ILocalizationServicesAccessor, ICultureAwareHtmlLocalizer
{
    private readonly DbHtmlLocalizerFactory _factory;

    /// <summary>
    /// Creates new instance of localizer
    /// </summary>
    /// <param name="factory">Factory to use</param>
    /// <param name="expressionHelper">Expression helper</param>
    public DbHtmlLocalizer(DbHtmlLocalizerFactory factory, ExpressionHelper expressionHelper) : base(factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        ExpressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
    }

    /// <summary>
    /// Changes language of the localizer
    /// </summary>
    /// <param name="language">Language to use</param>
    /// <returns>The <see cref="IHtmlLocalizer" /> with changed language.</returns>
    public IHtmlLocalizer ChangeLanguage(CultureInfo language)
    {
        return new DbHtmlLocalizer<TResource>(_factory.ChangeLanguage(language), ExpressionHelper);
    }

    /// <summary>
    /// Expression helper
    /// </summary>
    public ExpressionHelper ExpressionHelper { get; }
}
