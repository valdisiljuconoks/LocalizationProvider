// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Linq.Expressions;

namespace DbLocalizationProvider;

/// <summary>
/// Configuration class to play nicely with ASP.NET ModelMetadataProvider infrastructure
/// </summary>
public class ModelMetadataProvidersConfiguration
{
    /// <summary>
    /// Gets or sets a value to use cached version of ModelMetadataProvider.
    /// </summary>
    /// <value>
    /// <c>true</c> if cached ModelMetadataProvider should be used; otherwise, <c>false</c>.
    /// </value>
    public bool UseCachedProviders { get; set; }

    /// <summary>
    /// Gets or sets a value to replace ModelMetadataProvider to use new db localization system.
    /// </summary>
    /// <value>
    /// <c>true</c> if ModelMetadataProvider should be replaced; otherwise, <c>false</c>.
    /// </value>
    public bool ReplaceProviders { get; set; } = true;

    /// <summary>
    /// Set <c>true</c> to add translation returned from <see cref="RequiredFieldResource" /> for required fields.
    /// </summary>
    public bool MarkRequiredFields { get; set; } = false;

    /// <summary>
    /// If <see cref="MarkRequiredFields" /> is set to <c>true</c>, return of this method will be used to indicate required
    /// fields (added at the end of label).
    /// </summary>
    public Expression<Func<object>> RequiredFieldResource { get; set; }

    /// <summary>
    /// Gets or sets the callback for custom setup of the ModelMetadataProviders.
    /// </summary>
    public Action SetupCallback { get; set; }
}
