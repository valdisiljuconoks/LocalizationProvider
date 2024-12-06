// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Export;
using Localization.Xliff.OM.Core;
using Localization.Xliff.OM.Serialization;
using File = Localization.Xliff.OM.Core.File;

namespace DbLocalizationProvider.Xliff;

public class XliffResourceExporter : IResourceExporter
{
    public ExportResult Export(Dictionary<string, LocalizationResource> resources, Dictionary<string, string?[]>? parameters)
    {
        var sourceLang = parameters?["sourceLang"]?.FirstOrDefault();
        if (string.IsNullOrEmpty(sourceLang))
        {
            throw new ArgumentNullException(nameof(sourceLang));
        }

        var targetLang = parameters?["targetLang"]?.FirstOrDefault();
        if (string.IsNullOrEmpty(targetLang))
        {
            throw new ArgumentNullException(nameof(targetLang));
        }

        return Export(resources, CultureInfo.GetCultureInfo(sourceLang), CultureInfo.GetCultureInfo(targetLang));
    }

    public string FormatName => "XLIFF v2.0";

    public string ProviderId => "xliff";

    internal ExportResult Export(
        Dictionary<string, LocalizationResource> resources,
        CultureInfo fromLanguage,
        CultureInfo toLanguage)
    {
        ArgumentNullException.ThrowIfNull(resources);
        ArgumentNullException.ThrowIfNull(fromLanguage);
        ArgumentNullException.ThrowIfNull(toLanguage);

        var doc = new XliffDocument(fromLanguage.Name) { TargetLanguage = toLanguage.Name };

        var file = new File("f1");
        doc.Files.Add(file);

        var unit = new Unit("u1");
        file.Containers.Add(unit);

        foreach (var kv in resources)
        {
            var segment = new Segment(XmlConvert.EncodeNmToken(kv.Key))
            {
                Source = new Source(), Target = new Target()
            };

            segment.Source.Text.Add(new CDataTag(kv.Value.Translations.ByLanguage(fromLanguage.Name, false)));
            segment.Target.Text.Add(new CDataTag(kv.Value.Translations.ByLanguage(toLanguage.Name, false)));

            unit.Resources.Add(segment);
        }

        var dest = new MemoryStream();

        var settings = new XliffWriterSettings();
        settings.Validators.Clear();

        var writer = new XliffWriter(settings);

        writer.Serialize(dest, doc);
        dest.Position = 0;

        var reader = new StreamReader(dest);

        return new ExportResult(reader.ReadToEnd(),
                                "application/x-xliff+xml",
                                $"{fromLanguage.Name}-{toLanguage.Name}-{DateTime.UtcNow:yyyyMMdd}.xliff");
    }
}
