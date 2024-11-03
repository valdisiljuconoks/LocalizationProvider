// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using Microsoft.Extensions.Options;

namespace DbLocalizationProvider.AspNetCore.DataAnnotations;

/// <summary>
/// Model metadata helper.
/// </summary>
public class ModelMetadataLocalizationHelper
{
    private readonly IOptions<ConfigurationContext> _configurationContext;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly ILocalizationProvider _localizationProvider;

    /// <summary>
    /// Creates new instance of this class.
    /// </summary>
    /// <param name="localizationProvider">Localization provider itself.</param>
    /// <param name="keyBuilder">Resource key builder.</param>
    /// <param name="configurationContext">Configuration settings.</param>
    public ModelMetadataLocalizationHelper(
        ILocalizationProvider localizationProvider,
        ResourceKeyBuilder keyBuilder,
        IOptions<ConfigurationContext> configurationContext)
    {
        _localizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        _keyBuilder = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
        _configurationContext = configurationContext ?? throw new ArgumentNullException(nameof(configurationContext));
    }

    internal string GetTranslation(string resourceKey)
    {
        var result = resourceKey;
        if (!_configurationContext.Value.EnableLocalization())
        {
            return result;
        }

        var localizedDisplayName = _localizationProvider.GetString(resourceKey);
        result = localizedDisplayName;

        // for the legacy purposes - we need to look for this resource translation using display name
        // once again - this will make sure that existing XPath resources are still working
        if (localizedDisplayName != null && !_configurationContext.Value.ShouldLookupResource(localizedDisplayName))
        {
            result = _localizationProvider.GetString(localizedDisplayName);
        }

        // If other data annotations exists except for [Display], an exception is thrown when display name is ""
        // It should be null to avoid exception as ModelMetadata.GetDisplayName only checks for null and not String.Empty
        return string.IsNullOrWhiteSpace(localizedDisplayName) ? null : result;
    }

    internal string GetTranslation(Type containerType, string propertyName)
    {
        var resourceKey = _keyBuilder.BuildResourceKey(containerType, propertyName);

        return GetTranslation(resourceKey);
    }
}
