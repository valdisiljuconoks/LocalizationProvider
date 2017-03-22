using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace DbLocalizationProvider.MigrationTool.Tests
{
    public class XmlReaderSimpleTests
    {
        [Fact]
        public void EmptyList_NoEntries()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.Empty(resource);
        }

        [Fact]
        public void SingleResourceSingleLanguage_SingleEntry_TranslationInEnglish()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoption>This is display option</displayoption>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 1);

            var firstResource = resource.First();
            Assert.Equal("/displayoption", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var firstTranslation = firstResource.Translations.First();
            Assert.Equal("en", firstTranslation.Language);
            Assert.Equal("This is display option", firstTranslation.Value);
        }

        [Fact]
        public void SingleResourceWithWhitespaces_SingleEntry_TranslationWithoutWhitespaces()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoption>
        This is display option
    </displayoption>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 1);

            var firstResource = resource.First();
            Assert.Equal("/displayoption", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var firstTranslation = firstResource.Translations.First();
            Assert.Equal("en", firstTranslation.Language);
            Assert.Equal("This is display option", firstTranslation.Value);
        }

        [Fact]
        public void TwoResourcesSingleLanguage_AllEntries_TranslationInEnglish()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoption>This is display option</displayoption>
    <displayoption2>This is display option 2</displayoption2>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 2);

            var firstResource = resource[1];
            Assert.Equal("/displayoption2", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var firstTranslation = firstResource.Translations.First();
            Assert.Equal("en", firstTranslation.Language);
            Assert.Equal("This is display option 2", firstTranslation.Value);
        }

        [Fact]
        public void NestedResourceSingleLanguage_SingleEntry_TranslationInEnglish()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoptions>
        <displayoption>This is display option</displayoption>
    </displayoptions>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 1);

            var firstResource = resource.First();
            Assert.Equal("/displayoptions/displayoption", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var firstTranslation = firstResource.Translations.First();
            Assert.Equal("en", firstTranslation.Language);
            Assert.Equal("This is display option", firstTranslation.Value);
        }

        [Fact]
        public void NestedResourcesSingleLanguage_AllEntry_TranslationInEnglish()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoptions>
        <displayoption>This is display option</displayoption>
        <displayoption2>This is display option 2</displayoption2>
    </displayoptions>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 2);

            var firstResource = resource[1];
            Assert.Equal("/displayoptions/displayoption2", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var firstTranslation = firstResource.Translations.First();
            Assert.Equal("en", firstTranslation.Language);
            Assert.Equal("This is display option 2", firstTranslation.Value);
        }

        [Fact]
        public void NestedTwoLevelResourcesSingleLanguage_AllEntry_TranslationInEnglish()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoptions>
        <displayoption>This is display option</displayoption>
        <displayoptions2>
            <displayoption2>This is display option 2</displayoption2>
        </displayoptions2>
    </displayoptions>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 2);

            var firstResource = resource[1];
            Assert.Equal("/displayoptions/displayoptions2/displayoption2", firstResource.ResourceKey);

            Assert.Equal(1, firstResource.Translations.Count);

            var firstTranslation = firstResource.Translations.First();
            Assert.Equal("en", firstTranslation.Language);
            Assert.Equal("This is display option 2", firstTranslation.Value);
        }

        [Fact]
        public void SameKeyWithDifferentAttributeValue_TwoSeparateResourcesWithXPathInKey()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoptions>
        <displayoption name=""mobile"">Mobile</displayoption>
        <displayoption name=""desktop"">Desktop</displayoption>
    </displayoptions>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.True(resource.Count == 2);

            var firstResource = resource[0];
            Assert.Equal(@"/displayoptions/displayoption[@name=""mobile""]", firstResource.ResourceKey);
        }
[Fact]
        public void OneResourceWithAttributeValueOnParent_CorrectResourceKey()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoptions>
        <displayoption name=""mobile"">
            <name>Mobile</name>
        </displayoption>
    </displayoptions>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resource = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resource);
            Assert.Single(resource);

            var firstResource = resource[0];
            Assert.Equal(@"/displayoptions/displayoption[@name=""mobile""]/name", firstResource.ResourceKey);
        }

        [Fact]
        public void SingleResourceWithIgnoredAttribute_SingleResource()
        {
            var xmlSample = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<languages>
  <language name=""English"" id=""en"">
    <displayoptions>
        <displayoption file=""something.txt"">Mobile</displayoption>
    </displayoptions>
  </language>
</languages>";

            var parser = new XmlDocumentParser();
            var doc = XDocument.Parse(xmlSample);

            var resources = parser.ReadXml(doc).ToList();

            Assert.NotEmpty(resources);
            Assert.Single(resources);

            var firstResource = resources.First();
            Assert.Equal(@"/displayoptions/displayoption", firstResource.ResourceKey);
        }
    }
}
