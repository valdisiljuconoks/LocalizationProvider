// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Globalization;
using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// An <see cref="IViewLocalizer" /> implementation that derives the resource location from the executing view's
/// file path.
/// </summary>
public class DbViewLocalizer : ViewLocalizer, ILocalizationServicesAccessor, ICultureAwareHtmlLocalizer
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly DbHtmlLocalizerFactory _localizerFactory;

    /// <summary>
    /// Creates a new <see cref="ViewLocalizer" />.
    /// </summary>
    /// <param name="localizerFactory">The <see cref="IHtmlLocalizerFactory" />.</param>
    /// <param name="hostingEnvironment">The <see cref="IWebHostEnvironment" />.</param>
    /// <param name="expressionHelper">Expression helper</param>
    public DbViewLocalizer(
        DbHtmlLocalizerFactory localizerFactory,
        IWebHostEnvironment hostingEnvironment,
        ExpressionHelper expressionHelper)
        : base(localizerFactory, hostingEnvironment)
    {
        _localizerFactory = localizerFactory;
        _hostingEnvironment = hostingEnvironment;
        ExpressionHelper = expressionHelper;
    }

    /// <summary>
    /// Changes the language of the view localizer (USE WITH CAUTION! is this involves some magic to get it done).
    /// </summary>
    /// <param name="language">New language to set</param>
    /// <returns><see cref="IHtmlLocalizer" /> with changed language.</returns>
    public IHtmlLocalizer ChangeLanguage(CultureInfo language)
    {
        // capture initialized localizer field from the base
        var localizer = this.GetField<HtmlLocalizer, ViewLocalizer>("_localizer");

        // get underlying string localizer
        var underLyingLocalizer = localizer.GetField<DbStringLocalizer, HtmlLocalizer>("_localizer");
        if (underLyingLocalizer != null)
        {
            localizer.SetField<HtmlLocalizer>("_localizer", underLyingLocalizer.ChangeLanguage(language));
        }

        // create new instance of view localizer
        var dbViewLocalizer = new DbViewLocalizer(_localizerFactory, _hostingEnvironment, ExpressionHelper);

        // set back underlying localizer
        dbViewLocalizer.SetField<ViewLocalizer>("_localizer", localizer);

        // this all ceremony is required because we just can't new up instance of view localizer.
        // it's been contextualize during it's lifetime - so we need to "restore" state.
        // this is VERY HACK-ish and do not recommend anyone to use it. at all. forget about it.
        return dbViewLocalizer;
    }

    /// <summary>
    /// Expression helper
    /// </summary>
    public ExpressionHelper ExpressionHelper { get; }
}
