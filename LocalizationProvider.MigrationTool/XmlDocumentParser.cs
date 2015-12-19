using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TechFellow.LocalizationProvider.MigrationTool
{
    public class XmlDocumentParser
    {
        public IEnumerable<LocalizableResourceEntry> ReadXml(XDocument xmlDocument)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            var result = new List<LocalizableResourceEntry>();

            var allLanguageElements = xmlDocument.Elements("languages");

            foreach (var languageElement in allLanguageElements.Elements("language"))
            {
                var cultureName = languageElement.Attribute("name");
                var cultureId = languageElement.Attribute("id");

                ParseResource(languageElement.Elements(), cultureId.Value, cultureName.Value, result, string.Empty);
            }

            return result;
        }

        private static void ParseResource(IEnumerable<XElement> languageElements,
                                          string cultureId,
                                          string cultureName,
                                          ICollection<LocalizableResourceEntry> result,
                                          string keyPrefix)
        {
            foreach (var languageElement in languageElements)
            {
                if (languageElement.HasElements)
                {
                    ParseResource(languageElement.Elements(), cultureId, cultureName, result, keyPrefix + "/" + languageElement.Name.LocalName);
                }
                else
                {
                    if (string.IsNullOrEmpty(languageElement.Value))
                    {
                        continue;
                    }

                    var resourceKey = keyPrefix + "/" + languageElement.Name.LocalName;
                    var existingResource = result.FirstOrDefault(r => r.Key == resourceKey);

                    if (existingResource != null)
                    {
                        var existingTranslation = existingResource.Translations.FirstOrDefault(t => t.CultureId == cultureId);

                        if (existingTranslation != null)
                        {
                            throw new NotSupportedException($"Found duplicate translations for resource with key: {resourceKey}");
                        }

                        existingResource.Translations.Add(new ResourceTranslation(cultureId, cultureName, languageElement.Value));
                    }
                    else
                    {
                        var resourceEntry = new LocalizableResourceEntry(resourceKey);
                        resourceEntry.Translations.Add(new ResourceTranslation(cultureId, cultureName, languageElement.Value));
                        result.Add(resourceEntry);
                    }
                }
            }
        }
    }
}
