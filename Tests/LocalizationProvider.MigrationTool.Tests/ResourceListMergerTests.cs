using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TechFellow.LocalizationProvider.MigrationTool.Tests
{
    public class ResourceListMergerTests
    {
        [Fact]
        public void TwoEmptyLists_EmptyList()
        {
            var merger = new ResourceListMerger();
            var list1 = new List<ResourceEntry>();
            var list2 = new List<ResourceEntry>();

            var result = merger.Merge(list1, list2);

            Assert.Empty(result);
        }

        [Fact]
        public void OneListFilled_TheSameList()
        {
            var merger = new ResourceListMerger();
            var list1 = new List<ResourceEntry>
            {
                new ResourceEntry("key1")
            };

            var list2 = new List<ResourceEntry>();

            var result = merger.Merge(list1, list2);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void SecondListFilled_TheSameList()
        {
            var merger = new ResourceListMerger();
            var list1 = new List<ResourceEntry>();
            var list2 = new List<ResourceEntry>
            {
                new ResourceEntry("key1")
            };

            var result = merger.Merge(list1, list2);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void BothListsFilled_DifferentKeys_JoinedList()
        {
            var merger = new ResourceListMerger();
            var list1 = new List<ResourceEntry>
            {
                new ResourceEntry("key1")
            };

            var list2 = new List<ResourceEntry>
            {
                new ResourceEntry("key2")
            };

            var result = merger.Merge(list1, list2);

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void BothListsFilled_SameKeys_JoinedList()
        {
            var merger = new ResourceListMerger();
            var list1 = new List<ResourceEntry>
            {
                new ResourceEntry("key1")
            };

            var list2 = new List<ResourceEntry>
            {
                new ResourceEntry("key1")
            };

            var result = merger.Merge(list1, list2);

            Assert.NotEmpty(result);
            Assert.Single(result);
        }

        [Fact]
        public void BothListsFilledWithTranslations_SameKeys_JoinedList()
        {
            var merger = new ResourceListMerger();

            var resource1English = new ResourceEntry("key1");
            resource1English.Translations.Add(new ResourceTranslation("en", "English", "hello"));

            var resource1Norsk = new ResourceEntry("key1");
            resource1Norsk.Translations.Add(new ResourceTranslation("no", "Norsk", "hei"));

            var list1 = new List<ResourceEntry>
            {
                resource1English
            };

            var list2 = new List<ResourceEntry>
            {
                resource1Norsk
            };

            var result = merger.Merge(list1, list2);

            Assert.NotEmpty(result);
            Assert.Single(result);

            var finalResource = result.First();

            Assert.Equal(2, finalResource.Translations.Count);
        }
    }
}
