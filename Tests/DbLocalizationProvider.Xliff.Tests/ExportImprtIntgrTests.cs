using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace DbLocalizationProvider.Xliff.Tests
{
    public class ExportImprtIntgrTests
    {
        [Fact]
        public void ExportResourceWithForbiddenKeyName_NoExceptions()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("My.Resource.Key+ForbiddenPart")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "this is english text"
                                                       }
                                                   }
                                }
                            };

            var exporter = new Exporter();
            var parser = new FormatParser();

            var exportResult = exporter.Export(resources, new CultureInfo("en"), new CultureInfo("no"));
            Assert.NotNull(exportResult.SerializedData);

            var importResult = parser.Parse(exportResult.SerializedData);
            Assert.NotNull(importResult.Resources);

            Assert.Equal("My.Resource.Key+ForbiddenPart", importResult.Resources.First().ResourceKey);
        }
    }
}
