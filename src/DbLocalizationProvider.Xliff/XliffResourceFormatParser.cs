// Copyright (c) Valdis Iljuconoks. All rights reserved.
// Licensed under Apache-2.0. See the LICENSE file in the project root for more information

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Import;
using Localization.Xliff.OM.Core;
using Localization.Xliff.OM.Serialization;

namespace DbLocalizationProvider.Xliff;

public class FormatParser : IResourceFormatParser
{
    private static readonly string[] _extensions = { ".xlf", ".xliff" };

    public string FormatName => "XLIFF v2.0";

    public string[] SupportedFileExtensions => _extensions;

    public string ProviderId => "xliff";

    public ParseResult Parse(string fileContent)
    {
        var reader = new XliffReader();
        var doc = reader.Deserialize(AsStream(fileContent));

        var result = new List<LocalizationResource>();
        var languages = new List<string>();

        foreach (var file in doc.Files)
        {
            foreach (var container in file.Containers.OfType<Unit>())
            {
                foreach (var resource in container.Resources)
                {
                    var targetLanguage = resource.Target.Language;
                    var targetCulture = new CultureInfo(targetLanguage).Name;

                    var newResource = new LocalizationResource(XmlConvert.DecodeName(resource.Id), false);
                    newResource.Translations.AddRange(new List<LocalizationResourceTranslation>
                    {
                        new()
                        {
                            Language = targetCulture,
                            Value = resource.Target.Text.OfType<CDataTag>()
                                .FirstOrDefault()
                                ?.Text
                        }
                    });

                    result.Add(newResource);

                    if (!languages.Contains(targetCulture))
                    {
                        languages.Add(targetCulture);
                    }
                }
            }
        }

        return new ParseResult(result, languages.Select(l => new CultureInfo(l)).ToList());
    }

    private static Stream AsStream(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }
}
