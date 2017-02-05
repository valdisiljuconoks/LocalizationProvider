using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Import;
using Xunit;

namespace DbLocalizationProvider.Tests.ImporterTests
{
    public class ChangesDetectionTests
    {
        [Fact]
        public void ImportNewResource_OneAlreadyExists_OnlyInserts()
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
            var existing = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1" }
                    }
                }
            };

            var sut = new ResourceImporter();

            var result = sut.DetectChanges(incoming, existing);

            Assert.Equal(1, result.Count(c => c.ChangeType == ChangeType.Insert));
            Assert.Equal(0, result.Count(c => c.ChangeType == ChangeType.Update));
        }

        [Fact]
        public void ImportOne_CheckChangedLanguageWithDifferentTranslations()
        {
            var incoming = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1 CHANGED" },
                        new LocalizationResourceTranslation { Language = "no", Value = "Resurs 1 CHANGED" }
                    }
                }
            };
            var existing = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1 English" },
                        new LocalizationResourceTranslation { Language = "no", Value = "Resurs 1 Norsk" }
                    }
                }
            };
            var sut = new ResourceImporter();

            var result = sut.DetectChanges(incoming, existing);

            Assert.Equal(1, result.Count(c => c.ChangeType == ChangeType.Update));

            var firstChange = result.First();
            Assert.Equal(new[] { "en", "no" }, firstChange.ChangedLanguages.ToArray());
        }

        [Fact]
        public void ImportOne_WithNewTranslationLanguage()
        {
            var incoming = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1 English" },
                        new LocalizationResourceTranslation { Language = "no", Value = "Resurs 1" }
                    }
                }
            };
            var existing = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1 English" },
                    }
                }
            };
            var sut = new ResourceImporter();

            var result = sut.DetectChanges(incoming, existing);

            Assert.Equal(1, result.Count(c => c.ChangeType == ChangeType.Update));

            var firstChange = result.First();
            Assert.Equal(new[] { "no" }, firstChange.ChangedLanguages.ToArray());
        }

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
            var sut = new ResourceImporter();

            var result = sut.DetectChanges(incoming, new List<LocalizationResource>());

            Assert.Equal(incoming.Count, result.Count(c => c.ChangeType == ChangeType.Insert));
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
            var existing = new List<LocalizationResource>
            {
                new LocalizationResource("key1")
                {
                    Translations = new List<LocalizationResourceTranslation>
                    {
                        new LocalizationResourceTranslation { Language = "en", Value = "Resource 1 English" },
                        new LocalizationResourceTranslation { Language = "no", Value = "Resurs 1 Norsk" }
                    }
                }
            };

            var sut = new ResourceImporter();

            var result = sut.DetectChanges(incoming, existing);

            Assert.Equal(1, result.Count(c => c.ChangeType == ChangeType.Insert));
            Assert.Equal(1, result.Count(c => c.ChangeType == ChangeType.Update));
        }
    }
}
