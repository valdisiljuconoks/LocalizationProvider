using System;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace DbLocalizationProvider.MigrationTool.Tests
{
    public class XmlReaderMultipleLanguageTests
    {
        [Fact]
        public void SingleResourceTwoLanguages_SingleEntry_TranslationBothLanguages()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoption>This is display option</displayoption>
  </language>
  <language name=""Norsk"" id=""no"">
    <displayoption>Det er skjemalternativ</displayoption>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 1);

            var firstResource = resource.First();
            Assert.Equal("/displayoption", firstResource.ResourceKey);

            Assert.Equal(2, firstResource.Translations.Count);

            var norwegianTranslation = firstResource.Translations.First(t => t.Language == "no");
            Assert.Equal("no", norwegianTranslation.Language);
            Assert.Equal("Det er skjemalternativ", norwegianTranslation.Value);
        }

        [Fact]
        public void SingleResourceDuplicateLanguages_ExceptionThrown()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoption>This is display option</displayoption>
  </language>
  <language name=""English"" id=""en"">
    <displayoption>Whatever!</displayoption>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            Assert.Throws<NotSupportedException>(() => parser.ReadXml(doc).ToList());
        }

        [Fact]
        public void SingleResourceDuplicateLanguages_ExceptionNotThrownWithIgnoreDuplicateKeysOption()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
            <languages>
              <language name=""English"" id=""en"">
                <displayoption>This is display option</displayoption>
              </language>
              <language name=""English"" id=""en"">
                <displayoption>Whatever!</displayoption>
              </language>
            </languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc, true, "in-memory").ToList();

            var firstResource = resource.First();
            Assert.Equal("/displayoption", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var englishTranslation = firstResource.Translations.First(t => t.Language == "en");
            Assert.Equal("en", englishTranslation.Language);
            Assert.Equal("This is display option", englishTranslation.Value);
        }
    }
}
