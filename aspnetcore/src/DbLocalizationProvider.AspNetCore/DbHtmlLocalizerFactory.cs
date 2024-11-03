// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// A factory that creates <see cref="IHtmlLocalizer" /> instances.
/// </summary>
public class DbHtmlLocalizerFactory : IHtmlLocalizerFactory
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly CultureInfo _language;
    private readonly ILocalizationProvider _localizationProvider;
    private readonly DbStringLocalizerFactory _localizerFactory;

    /// <summary>
    /// Creates new instance of this class
    /// </summary>
    /// <param name="localizerFactory">Localizer factory</param>
    /// <param name="localizationProvider">Localization provider</param>
    /// <param name="expressionHelper">Expression builder</param>
    public DbHtmlLocalizerFactory(
        DbStringLocalizerFactory localizerFactory,
        ILocalizationProvider localizationProvider,
        ExpressionHelper expressionHelper)
    {
        _localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        _expressionHelper = expressionHelper ?? throw new ArgumentNullException(nameof(expressionHelper));
    }

    /// <summary>
    /// Creates new instance of this class
    /// </summary>
    /// <param name="language">Specify a language to be used for the factory</param>
    /// <param name="localizerFactory">Localizer factory</param>
    /// <param name="localizationProvider">Localization provider</param>
    /// <param name="expressionHelper">Expression builder</param>
    private DbHtmlLocalizerFactory(
        CultureInfo language,
        DbStringLocalizerFactory localizerFactory,
        ILocalizationProvider localizationProvider,
        ExpressionHelper expressionHelper) : this(localizerFactory, localizationProvider, expressionHelper)
    {
        _language = language ?? throw new ArgumentNullException(nameof(language));
    }

    /// <summary>
    /// Creates an <see cref="IHtmlLocalizer" />.
    /// </summary>
    /// <param name="baseName">The base name of the resource to load strings from.</param>
    /// <param name="location">The location to load resources from.</param>
    /// <returns>The <see cref="IHtmlLocalizer" />.</returns>
    public IHtmlLocalizer Create(string baseName, string location)
    {
        if (baseName == null)
        {
            throw new ArgumentNullException(nameof(baseName));
        }

        if (location == null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        return new DbHtmlLocalizer(_localizerFactory.Create(baseName, location), _expressionHelper);
    }

    /// <summary>
    /// Creates an <see cref="IHtmlLocalizer" /> using the <see cref="System.Reflection.Assembly" /> and
    /// <see cref="Type.FullName" /> of the specified <see cref="Type" />.
    /// </summary>
    /// <param name="resourceSource">The <see cref="Type" />.</param>
    /// <returns>The <see cref="IHtmlLocalizer" />.</returns>
    public IHtmlLocalizer Create(Type resourceSource)
    {
        if (resourceSource == null)
        {
            throw new ArgumentNullException(nameof(resourceSource));
        }

        return new DbHtmlLocalizer(_localizerFactory.Create(resourceSource), _expressionHelper);
    }

    /// <summary>
    /// Changes language of given HtmlLocalizer factory
    /// </summary>
    /// <param name="language">Language to change to</param>
    /// <returns>The <see cref="IHtmlLocalizerFactory" />.</returns>
    public DbHtmlLocalizerFactory ChangeLanguage(CultureInfo language)
    {
        return new DbHtmlLocalizerFactory(language,
                                          _localizerFactory.ChangeLanguage(language),
                                          _localizationProvider,
                                          _expressionHelper);
    }
}
