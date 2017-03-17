using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DbLocalizationProvider.MigrationTool
{
    internal class XmlDocumentParser
    {
        public ICollection<LocalizationResource> ReadXml(XDocument xmlDocument)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            var result = new List<LocalizationResource>();

            var allLanguageElements = xmlDocument.Elements("languages");

            foreach (var languageElement in allLanguageElements.Elements("language"))
            {
                var cultureId = languageElement.Attribute("id");
                ParseResource(languageElement.Elements(), cultureId.Value, result, string.Empty);
            }

            return result;
        }

        private static void ParseResource(IEnumerable<XElement> resourceElements,
                                          string cultureName,
                                          ICollection<LocalizationResource> result,
                                          string keyPrefix)
        {
            foreach (var element in resourceElements)
            {
                var resourceKey = keyPrefix + "/" + element.Name.LocalName;
                if (element.Attributes().Any(a => a.Name.LocalName != "comment"
                                                  && a.Name.LocalName != "file"
                                                  && a.Name.LocalName != "notapproved"
                                                  && a.Name.LocalName != "changed"))
                {
                    var attribute = element.FirstAttribute;
                    resourceKey += $"[@{attribute.Name.LocalName}=\"{attribute.Value}\"]";
                }

                if (element.HasElements)
                {
                    ParseResource(element.Elements(), cultureName, result, resourceKey);
                }
                else
                {
                    var resourceTranslation = element.Value.Trim();

                    if (string.IsNullOrEmpty(resourceTranslation))
                    {
                        continue;
                    }

                    var existingResource = result.FirstOrDefault(r => r.ResourceKey == resourceKey);

                    if (existingResource != null)
                    {
                        var existingTranslation = existingResource.Translations.FirstOrDefault(t => t.Language == cultureName);

                        if (existingTranslation != null)
                        {
                            throw new NotSupportedException($"Found duplicate translations for resource with key: {resourceKey} for culture: {cultureName}");
                        }

                        existingResource.Translations.Add(new LocalizationResourceTranslation
                                                          {
                                                              Language = cultureName,
                                                              Value = resourceTranslation
                                                          });
                    }
                    else
                    {
                        var resourceEntry = new LocalizationResource
                                            {
                                                ResourceKey = resourceKey,
                                                ModificationDate = DateTime.Now,
                                                Author = "migration-tool"
                                            };

                        resourceEntry.Translations.Add(new LocalizationResourceTranslation
                                                       {
                                                           Language = cultureName,
                                                           Value = resourceTranslation
                                                       });

                        result.Add(resourceEntry);
                    }
                }
            }
        }
    }
}
