// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using DbLocalizationProvider.Internal;

namespace DbLocalizationProvider.AspNetCore;

/// <summary>
/// Interface is more or less used as marker interface to understand whether type has access to various localization provider required services
/// </summary>
public interface ILocalizationServicesAccessor
{
    /// <summary>
    /// Expression helper to be used to walk lambdas
    /// </summary>
    ExpressionHelper ExpressionHelper { get; }
}
