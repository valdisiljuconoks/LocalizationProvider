using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using DbLocalizationProvider.Export;
using Localization.Xliff.OM.Core;
using Localization.Xliff.OM.Serialization;
using File = Localization.Xliff.OM.Core.File;

namespace DbLocalizationProvider.Xliff
{
    public class Exporter : IResourceExporter
    {
        public ExportResult Export(ICollection<LocalizationResource> resources, NameValueCollection parameters)
        {
            var sourceLang = parameters["sourceLang"];
            if(string.IsNullOrEmpty(sourceLang))
                throw new ArgumentNullException("Key `sourceLang` not found parameters");
            
            var targetLang = parameters["targetLang"];
            if(string.IsNullOrEmpty(targetLang))
                throw new ArgumentNullException("Key `targetLang` not found parameters");
            
            // NOTE: legacy reosurces could not be exported as they contain illegal characters in keys
            return Export(resources.Where(r => !r.ResourceKey.StartsWith("/")).ToList(), new CultureInfo(sourceLang), new CultureInfo(targetLang));
        }
        
        internal ExportResult Export(ICollection<LocalizationResource> resources, CultureInfo fromLanguage, CultureInfo toLanguage)
        {
            if(resources == null)
                throw new ArgumentNullException(nameof(resources));
            if(fromLanguage == null)
                throw new ArgumentNullException(nameof(fromLanguage));
            if(toLanguage == null)
                throw new ArgumentNullException(nameof(toLanguage));

            var doc = new XliffDocument(fromLanguage.Name)
                      {
                          TargetLanguage = toLanguage.Name
                      };

            var file = new File("f1");
            doc.Files.Add(file);

            var unit = new Unit("u1");
            file.Containers.Add(unit);

            foreach(var resource in resources)
            {
                var segment = new Segment(resource.ResourceKey)
                              {
                                  Source = new Source(),
                                  Target = new Target()
                              };

                segment.Source.Text.Add(new CDataTag(resource.Translations.ByLanguage(fromLanguage)));
                segment.Target.Text.Add(new CDataTag(resource.Translations.ByLanguage(toLanguage)));

                unit.Resources.Add(segment);
            }

            var dest = new MemoryStream();

            var settings = new XliffWriterSettings();
            settings.Validators.Clear();

            var writer = new XliffWriter(settings);

            writer.Serialize(dest, doc);
            dest.Position = 0;

            var reader = new StreamReader(dest);

            return new ExportResult(reader.ReadToEnd(), "application/x-xliff+xml", $"{fromLanguage.Name}-{toLanguage.Name}-{DateTime.UtcNow:yyyyMMdd}.xliff");
        }

        public string FormatName => "XLIFF v2.0";
        public string ProviderId => "xliff";
    }
}
