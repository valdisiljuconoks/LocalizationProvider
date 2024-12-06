// Copyright (c) Mattias Olsson, Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Export;

namespace DbLocalizationProvider.Csv;

/// <summary>
/// CSV export implementation
/// </summary>
public class CsvResourceExporter : IResourceExporter
{
    private readonly Func<ICollection<CultureInfo>>? _languagesFactory;

    public CsvResourceExporter() : this(null) { }

    public CsvResourceExporter(Func<ICollection<CultureInfo>>? languagesFactory)
    {
        _languagesFactory = languagesFactory;
    }

    /// <summary>
    /// Gets the name of the export format (this will be visible on menu).
    /// </summary>
    public string FormatName => "CSV";

    /// <summary>
    /// Gets the export provider identifier.
    /// </summary>
    public string ProviderId => "csv";

    /// <summary>
    /// Exports the specified resources.
    /// </summary>
    /// <param name="resources">The resources.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// Result of the export
    /// </returns>
    public ExportResult Export(Dictionary<string, LocalizationResource> resources, Dictionary<string, string?[]>? parameters)
    {
        var records = new List<object>();
        var languages = GetLanguages(resources);

        foreach (var kv in resources.OrderBy(x => x.Key))
        {
            dynamic record = new ExpandoObject();

            record.ResourceKey = kv.Key;

            foreach (var language in languages)
            {
                var translation = kv.Value.Translations.ByLanguage(language.Name, false);
                AddProperty(record, language.Name, translation);
            }

            records.Add(record);
        }

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);

        using var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, Encoding.UTF8);
        using var csv = new CsvWriter(streamWriter, csvConfig);
        
        csv.WriteRecords((IEnumerable)records);
        streamWriter.Flush();
        var bytes = stream.ToArray();
        var csvContent = Encoding.UTF8.GetString(bytes);
        var fileName = $"localization-resources-{DateTime.UtcNow:yyyyMMdd}.csv";

        return new ExportResult(csvContent, "text/csv", fileName);
    }

    private ICollection<CultureInfo> GetLanguages(Dictionary<string, LocalizationResource> resources)
    {
        if (_languagesFactory != null)
        {
            return _languagesFactory();
        }

        return resources
            .Select(kv => kv.Value)
            .SelectMany(x => x.Translations)
            .Select(x => x.Language)
            .Distinct()
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(TryGetCulture)
            .Where(x => x != null)
            .ToList();
    }

    private static CultureInfo? TryGetCulture(string cultureName)
    {
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            return culture;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void AddProperty(ExpandoObject record, string languageName, string translation)
    {
        var recordDict = record as IDictionary<string, object>;
        recordDict[languageName] = translation;
    }
}
