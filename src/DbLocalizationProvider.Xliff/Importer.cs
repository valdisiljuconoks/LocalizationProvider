using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbLocalizationProvider.Import;
using Localization.Xliff.OM.Core;
using Localization.Xliff.OM.Serialization;

namespace DbLocalizationProvider.Xliff
{
    public class Importer : IResourceImporter
    {
        private static readonly string[] _extensions = { ".xlf", ".xliff" };

        public string FormatName => "XLIFF v2.0";

        public string[] SupportedFileExtensions => _extensions;

        public string ProviderId => "xliff";

        public ICollection<LocalizationResource> Parse(string fileContent)
        {
            var reader = new XliffReader();
            var doc = reader.Deserialize(AsStream(fileContent));

            var result = new List<LocalizationResource>();

            foreach(var file in doc.Files)
            {
                foreach(var container in file.Containers.OfType<Unit>())
                {
                    foreach(var resource in container.Resources)
                    {
                        result.Add(new LocalizationResource(resource.Id)
                                   {
                                       Translations = new List<LocalizationResourceTranslation>
                                                      {
                                                          new LocalizationResourceTranslation
                                                          {
                                                              Language = resource.Target.Language,
                                                              Value = resource.Target.Text.OfType<CDataTag>().FirstOrDefault()?.Text
                                                          }
                                                      }
                                   });
                    }
                }
            }

            return result;
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
}