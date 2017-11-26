using System.Collections.Generic;
using Xunit;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler.Tests
{
    public class JsonConverterTests
    {
        [Fact]
        public void VariousResources_WithMixedKeyNames()
        {
            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("/another/mixmatch/key")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english"
                        }
                    }
                },
                new LocalizationResource("This.Is.Resource.Key")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                }
            };
            var sut = new ResourceJsonConverter();

            var resourcesAsJson = sut.Convert(resources, "en");

            Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);

            Assert.Equal(1, resourcesAsJson.Count);
        }

        [Fact]
        public void VariousResources_WithNorskTranslation_RequestedEnglish_NoResults()
        {
            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.AnotherResource.Key")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                }
            };
            var sut = new ResourceJsonConverter();

            var resourcesAsJson = sut.Convert(resources, "en");

            Assert.Equal(0, resourcesAsJson.Count);
        }

        [Fact]
        public void VariousResources_WithSharedRootKeyName()
        {
            var sut = new ResourceJsonConverter();

            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.Resource.AnotherKey")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is another norsk"
                        }
                    }
                },
                new LocalizationResource("This.Is.YetAnotherResource.AnotherKey")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is another english 2"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is another norsk 2"
                        }
                    }
                }
            };

            var resourcesAsJson = sut.Convert(resources, "en");

            Assert.Equal("this is english", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
        }

        [Fact]
        public void ResoureceWithMultipleTranslations_ReturnRequestedTranslation()
        {
            var sut = new ResourceJsonConverter();

            var resources = new List<LocalizationResource>
            {
                new LocalizationResource("This.Is.Resource.Key")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation
                        {
                            Language = "en",
                            Value = "this is english"
                        },
                        new LocalizationResourceTranslation
                        {
                            Language = "no",
                            Value = "this is norsk"
                        }
                    }
                }
            };

            var resourcesAsJson = sut.Convert(resources, "no");

            Assert.Equal("this is norsk", resourcesAsJson["This"]["Is"]["Resource"]["Key"]);
        }
    }
}
