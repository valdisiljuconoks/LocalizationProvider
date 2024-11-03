using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using Xunit;

namespace DbLocalizationProvider.Xliff.Tests
{
    public class TestXliffExport
    {
        [Fact]
        public void SimpleTest_EmptyDocument()
        {
            var first = new LocalizationResource("My.Resource.Key", false);
            first.Translations.Add(
                new LocalizationResourceTranslation
                {
                    Language = "en",
                    Value = "this is english text"
                });
            first.Translations.Add(
                new LocalizationResourceTranslation
                {
                    Language = "no",
                    Value = "det er tekst i norsk"
                });

            var second = new LocalizationResource("My.Resource.AnotherKey", false);
            second.Translations.Add(
                new LocalizationResourceTranslation
                {
                    Language = "en",
                    Value = "this is another english text"
                });
            second.Translations.Add(
                new LocalizationResourceTranslation
                {
                    Language = "no",
                    Value = "det er andre tekst i norsk"
                });

            var resources = new List<LocalizationResource>
                            {
                                first,
                                second
                            };

            var sut = new XliffResourceExporter();

            var result = sut.Export(resources, new Dictionary<string, string[]>(){
                {"sourceLang", ["en"]},
                {"targetLang", ["no"]}
            });

            Assert.NotNull(result);
        }

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
            var resources = new List<LocalizationResource>
                            {
                                first
                            };

            var sut = new XliffResourceExporter();
            var result = sut.Export(resources, new Dictionary<string, string[]>(){
                {"sourceLang", ["en"]},
                {"targetLang", ["no"]}
            });

            Assert.NotNull(result);
        }
    }
}
