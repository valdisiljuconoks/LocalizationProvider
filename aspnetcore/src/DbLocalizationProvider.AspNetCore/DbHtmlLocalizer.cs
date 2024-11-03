// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Internal;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// HtmlLocalizer with access to required services
/// </summary>
public class DbHtmlLocalizer : HtmlLocalizer, ILocalizationServicesAccessor
{
    /// <summary>
    /// Creates new instance of this class.
    /// </summary>
    /// <param name="stringLocalizer">Underlying string localizer.</param>
    /// <param name="expressionHelper">Expression helper</param>
    public DbHtmlLocalizer(IStringLocalizer stringLocalizer, ExpressionHelper expressionHelper) : base(stringLocalizer)
    {
        ExpressionHelper = expressionHelper;
    }

    /// <summary>
    /// Expression helper
    /// </summary>
    public ExpressionHelper ExpressionHelper { get; }
}
