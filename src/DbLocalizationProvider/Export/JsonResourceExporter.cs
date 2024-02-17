// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using Newtonsoft.Json;

namespace DbLocalizationProvider.Export;

/// <summary>
/// JSON format exporter.
/// </summary>
/// <seealso cref="DbLocalizationProvider.Export.IResourceExporter" />
public class JsonResourceExporter : IResourceExporter
{
    internal static JsonSerializerSettings DefaultSettings
    {
        get
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new JsonDefaultContractResolver(),
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Culture = CultureInfo.InvariantCulture,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
            };

            return settings;
        }
    }

    /// <summary>
    /// Exports the specified resources.
    /// </summary>
    /// <param name="resources">The resources.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// Result of the export
    /// </returns>
    public ExportResult Export(ICollection<LocalizationResource> resources, IDictionary<string, string[]> parameters)
    {
        return new ExportResult(JsonConvert.SerializeObject(resources, DefaultSettings),
                                "application/json",
                                $"localization-resources-{DateTime.UtcNow:yyyyMMdd}.json");
    }

    /// <summary>
    /// Gets the name of the export format (this will be visible on menu).
    /// </summary>
    public string FormatName => "JSON";

    /// <summary>
    /// Gets the export provider identifier.
    /// </summary>
    public string ProviderId => "json";

    /// <summary>
    /// Deserializes the specified string value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stringValue">The string value.</param>
    /// <returns></returns>
    public T Deserialize<T>(string stringValue) where T : class
    {
        return JsonConvert.DeserializeObject<T>(stringValue, DefaultSettings);
    }
}
