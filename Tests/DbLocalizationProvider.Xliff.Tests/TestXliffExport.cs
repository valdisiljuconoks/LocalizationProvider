using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace DbLocalizationProvider.Xliff.Tests
{
    public class TestXliffExport
    {
        [Fact]
        public void SimpleTest_EmptyDocument()
        {
            var resources = new List<LocalizationResource>
                            {
                                new LocalizationResource("My.Resource.Key")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "this is english text"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "no",
                                                           Value = "det er tekst i norsk"
                                                       }
                                                   }
                                },
                                new LocalizationResource("My.Resource.AnotherKey")
                                {
                                    Translations = new List<LocalizationResourceTranslation>
                                                   {
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "en",
                                                           Value = "this is another english text"
                                                       },
                                                       new LocalizationResourceTranslation
                                                       {
                                                           Language = "no",
                                                           Value = "det er andre tekst i norsk"
                                                       }
                                                   }
                                }
                            };

            var sut = new Exporter();

            var result = sut.Export(resources, new CultureInfo("en"), new CultureInfo("no"));

            Assert.NotNull(result);
        }

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

            var sut = new Exporter();
            var result = sut.Export(resources, new CultureInfo("en"), new CultureInfo("no"));

            Assert.NotNull(result);
        }
    }
}
