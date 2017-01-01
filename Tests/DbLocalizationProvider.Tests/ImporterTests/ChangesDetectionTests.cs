using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Import;
using DbLocalizationProvider.Queries;
using Xunit;

namespace DbLocalizationProvider.Tests.ImporterTests
{
    public class ChangesDetectionTests
    {
        [Fact]
        public void ImportSome_EmptyDatabase_OnlyInserts()
        {
            var incoming = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1" }
                    }
                },
                new LocalizationResource("key2")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 2" }
                    }
                }
            };

            ConfigurationContext.Setup(cfg =>
                                       {
                                           cfg.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<NoResourcesQueryHandler>();
                                       });

            var sut =  new ResourceImporter();

            var result = sut.DetectChanges(incoming);


            Assert.Equal(incoming.Count, result.Count(c => c.Change == ChangeType.Insert));
        }

        [Fact]
        public void ImportSome_TheSameInDatabase_OnlyInserts()
        {
            var incoming = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1" }
                    }
                },
                new LocalizationResource("key2")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 2" }
                    }
                }
            };

            ConfigurationContext.Setup(cfg =>
                                       {
                                           cfg.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<SomeResourcesQueryHandler>();
                                       });

            var sut =  new ResourceImporter();

            var result = sut.DetectChanges(incoming);


            Assert.Equal(1, result.Count(c => c.Change == ChangeType.Insert));
            Assert.Equal(0, result.Count(c => c.Change == ChangeType.Update));
        }

        [Fact]
        public void ImportSome_TheSameWithDifferentTranslationInDatabase_InsertsAndUpdates()
        {
            var incoming = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1" }
                    }
                },
                new LocalizationResource("key2")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 2 - Changed!" }
                    }
                }
            };

            ConfigurationContext.Setup(cfg =>
                                       {
                                           cfg.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler<SomeResourcesQueryHandler>();
                                       });

            var sut =  new ResourceImporter();

            var result = sut.DetectChanges(incoming);


            Assert.Equal(1, result.Count(c => c.Change == ChangeType.Insert));
            Assert.Equal(1, result.Count(c => c.Change == ChangeType.Update));
        }
    }


    public class NoResourcesQueryHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            return new List<LocalizationResource>();
        }
    }

    public class SomeResourcesQueryHandler : IQueryHandler<GetAllResources.Query, IEnumerable<LocalizationResource>>
    {
        public IEnumerable<LocalizationResource> Execute(GetAllResources.Query query)
        {
            return new List<LocalizationResource>
            {
                new LocalizationResource("key2")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 2" }
                    }
                }
            };
        }
    }
}
