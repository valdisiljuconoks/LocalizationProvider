// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Export;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Import;

/// <summary>
/// Resource parser that talks in JSON
/// </summary>
/// <seealso cref="DbLocalizationProvider.Import.IResourceFormatParser" />
public class JsonResourceFormatParser : IResourceFormatParser
{
    private static readonly string[] _extensions = [".json"];

    /// <summary>
    /// Gets the name of the format.
    /// </summary>
    public string FormatName => "JSON";

    /// <summary>
    /// Gets the supported file extensions.
    /// </summary>
    public string[] SupportedFileExtensions => _extensions;

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string ProviderId => "json";

    /// <summary>
    /// Parses the specified file content.
    /// </summary>
    /// <param name="fileContent">Content of the file.</param>
    /// <returns>
    /// Returns list of resources from the file
    /// </returns>
    public ParseResult Parse(string fileContent)
    {
        var result =
            JsonConvert.DeserializeObject<ICollection<LocalizationResource>>(
                    fileContent,
                    JsonResourceExporter.DefaultSettings)
                .Where(_ => _.Translations != null && _.Translations.Count > 0)
                .ToList();

        var detectedLanguages = result
            .SelectMany(r => r.Translations.Where(_ => _ != null).Select(_ => _.Language))
            .Distinct()
            .Where(_ => !string.IsNullOrEmpty(_));

        return new ParseResult(result, detectedLanguages.Select(CultureInfo.GetCultureInfo).ToList());
    }
}
