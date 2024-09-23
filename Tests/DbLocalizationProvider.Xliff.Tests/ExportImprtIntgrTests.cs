using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using Xunit;

namespace DbLocalizationProvider.Xliff.Tests
{
    public class ExportImprtIntgrTests
    {
        [Fact]
        public void ExportResourceWithForbiddenKeyName_NoExceptions()
        {
            var first = new LocalizationResource("My.Resource.Key+ForbiddenPart", false);
            first.Translations.Add(
                new LocalizationResourceTranslation
                {
                    Language = "en",
                    Value = "this is english text"
                });

            var resources = new List<LocalizationResource>(){
                first
            };

            var exporter = new XliffResourceExporter();
            var parser = new FormatParser();

            var exportResult = exporter.Export(resources, new Dictionary<string, string[]>(){
                {"sourceLang", ["en"]},
                {"targetLang", ["no"]}
            });
            Assert.NotNull(exportResult.SerializedData);

            var importResult = parser.Parse(exportResult.SerializedData);
            Assert.NotNull(importResult.Resources);

            Assert.Equal("My.Resource.Key+ForbiddenPart", importResult.Resources.First().ResourceKey);
        }
    }
}
