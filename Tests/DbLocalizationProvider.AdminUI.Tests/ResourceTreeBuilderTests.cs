using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace DbLocalizationProvider.AdminUI.Tests
{
    public class ResourceTreeBuilderTests
    {
        [Theory]
        [InlineData(new[]
                    {
                        "MyNamespace.MyProject.MyResource",
                        "MyNamespace.MyProject.AnotherResource",
                        "OtherNamespace.MyProject.OtherResource"
                    },
            false)]
        [InlineData(new[]
                    {
                        "/MyNamespace/MyProject/MyResource",
                        "/MyNamespace/MyProject/AnotherResource",
                        "/OtherNamespace/MyProject/OtherResource"
                    },
            true)]
        public void MultipleResources_HavingSimilarKeyFragment_RegisteredUnderCorrectParent(IEnumerable<string> resourceKeys, bool isLegacyMode)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>();

            foreach(var resourceKey in resourceKeys)
            {
                model.Add(new ResourceListItem(resourceKey,
                                               new List<ResourceItem>(new[]
                                                                      {
                                                                          new ResourceItem(resourceKey,
                                                                                           "sample translation",
                                                                                           new CultureInfo("en"))
                                                                      }),
                                               true,
                                               false));
            }

            var result = sut.BuildTree(model, isLegacyMode);

            Assert.NotNull(result);
            Assert.Equal(7, result.Count);
        }

        [Theory]
        [InlineData("MyNamespace.MyProject.MyResource", false)]
        [InlineData("/MyNamespace/MyProject/MyResource", true)]
        public void SingleResource_SplitIntoKeyFragments_AllRegistered(string resourceKey, bool isLegacyModeEnabled)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>
                        {
                            new ResourceListItem(resourceKey,
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem(resourceKey,
                                                                                             "sample translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false)
                        };

            var result = sut.BuildTree(model, isLegacyModeEnabled);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.False(result.First().IsLeaf);
            Assert.False(result.Skip(1).First().IsLeaf);
            Assert.True(result.Skip(2).First().IsLeaf);
        }

        [Theory]
        [InlineData(
            new[]
            {
                "MyNamespace.MyProject.AnotherResource",
                "MyNamespace.MyProject.MyResource"
            },
            new[]
            {
                true,
                false
            },
            false)]
        [InlineData(
            new[]
            {
                "/MyNamespace/MyProject/AnotherResource",
                "/MyNamespace/MyProject/MyResource"
            },
            new[]
            {
                true,
                false
            },
            true)]
        public void TwoResources_SameRootKey_OneHidden_RootIsNotHidden(string[] resourceKeys, bool[] visibilityFlags, bool isLecagyMode)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>();

            for(var i = 0; i < resourceKeys.Length; i++)
            {
                var resourceKey = resourceKeys[i];
                model.Add(new ResourceListItem(resourceKey,
                                               new List<ResourceItem>(new[]
                                                                      {
                                                                          new ResourceItem(resourceKey,
                                                                                           "another translation",
                                                                                           new CultureInfo("en"))
                                                                      }),
                                               true,
                                               visibilityFlags[i]));
            }

            var result = sut.BuildTree(model, isLecagyMode);
            var shareRootKey = result.Single(r => r.KeyFragment == "MyNamespace");
            var shareParentKey = result.Single(r => r.KeyFragment == "MyProject");

            Assert.False(shareRootKey.IsHidden, "Shared root key is hidden");
            Assert.False(shareParentKey.IsHidden);
        }

        [Theory]
        [InlineData(
            new[]
            {
                "MyNamespace.MyProject.AnotherResource",
                "AnotherNamespace.MyProject.MyResource"
            },
            new[]
            {
                true,
                false
            },
            false,
            "MyNamespace.MyProject",
            "AnotherNamespace.MyProject")]
        [InlineData(
            new[]
            {
                "/MyNamespace/MyProject/AnotherResource",
                "/AnotherNamespace/MyProject/MyResource"
            },
            new[]
            {
                true,
                false
            },
            true,
            "MyNamespace/MyProject",
            "AnotherNamespace/MyProject")]
        public void TwoResources_SimilarParent_DifferentRootKeys_OneHidden_ParentIsHidden(
            string[] resourceKeys,
            bool[] visibilityFlags,
            bool isLecagyMode,
            string parentResourcePath1,
            string parentResourcePath2)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>();

            for(var i = 0; i < resourceKeys.Length; i++)
            {
                model.Add(new ResourceListItem(resourceKeys[i],
                                               new List<ResourceItem>(new[]
                                                                      {
                                                                          new ResourceItem(resourceKeys[i],
                                                                                           "another translation",
                                                                                           new CultureInfo("en"))
                                                                      }),
                                               true,
                                               visibilityFlags[i]));
            }

            var result = sut.BuildTree(model, isLecagyMode);
            var similarParentKey = result.Single(r => r.Path == parentResourcePath1);
            var anotherSimilarParentKey = result.Single(r => r.Path == parentResourcePath2);

            Assert.True(similarParentKey.IsHidden);
            Assert.False(anotherSimilarParentKey.IsHidden);
        }

        [Theory]
        [InlineData(
            new[]
                    {
                        "MyNamespace.MyProject.AnotherResource",
                        "MyNamespace.MyProject.MyResource",
                        "MyNamespace.MyProject.HiddenResource"
                    },
            new[]
                {
                    true,
                    false,
                    true
                },
            false)]
        [InlineData(
            new[]
                    {
                        "/MyNamespace/MyProject/AnotherResource",
                        "/MyNamespace/MyProject/MyResource",
                        "/MyNamespace/MyProject/HiddenResource"
                    },
            new[]
                {
                    true,
                    false,
                    true
                },
            true)]
        public void CoupleResources_SameRootKey_TwoHidden_RootIsNotHidden(string[] resourceKeys, bool[] visibilityFlags, bool isLegacyMode)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>();

            for(var i = 0; i < resourceKeys.Length; i++)
            {
                model.Add(new ResourceListItem(resourceKeys[i],
                                               new List<ResourceItem>(new[]
                                                                      {
                                                                          new ResourceItem(resourceKeys[i],
                                                                                           "another translation",
                                                                                           new CultureInfo("en"))
                                                                      }),
                                               true,
                                               visibilityFlags[i]));
            }

            var result = sut.BuildTree(model, isLegacyMode);
            var shareRootKey = result.Single(r => r.KeyFragment == "MyNamespace");
            var shareParentKey = result.Single(r => r.KeyFragment == "MyProject");

            Assert.False(shareRootKey.IsHidden);
            Assert.False(shareParentKey.IsHidden);
        }

        [Theory]
        [InlineData(
            new[]
            {
                "MyNamespace.MyProject.MyResource",
                "MyNamespace.MyProject.AnotherResource"
            },
            false)]
        [InlineData(
            new[]
            {
                "/MyNamespace/MyProject/MyResource",
                "/MyNamespace/MyProject/AnotherResource"
            },
            true)]
        public void TwoResources_UnderSingleRootKey_BothRegistered(string[] resourceKeys, bool isLegacyMode)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>();

            foreach(var resourceKey in resourceKeys)
            {
                model.Add(new ResourceListItem(resourceKey,
                                               new List<ResourceItem>(new[]
                                                                      {
                                                                          new ResourceItem(resourceKey,
                                                                                           "sample translation",
                                                                                           new CultureInfo("en"))
                                                                      }),
                                               true,
                                               false));
            }

            var result = sut.BuildTree(model, isLegacyMode);

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
        }

        [Theory]
        [InlineData(
            new[]
            {
                "MyNamespace.MyProject.MyResource",
                "/MyNamespace/MyOtherProject/AnotherResource"
            },
            true)]
        [InlineData(
            new[]
            {
                "/MyNamespace/MyProject/MyResource",
                "MyNamespace.MyOtherProject.AnotherResource"
            },
            true)]
        public void MixedResources_AllRegistered(string[] resourceKeys, bool isLegacyMode)
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>();

            foreach(var resourceKey in resourceKeys)
            {
                model.Add(new ResourceListItem(resourceKey,
                                               new List<ResourceItem>(new[]
                                                                      {
                                                                          new ResourceItem(resourceKey,
                                                                                           "sample translation",
                                                                                           new CultureInfo("en"))
                                                                      }),
                                               true,
                                               false));
            }

            var result = sut.BuildTree(model, isLegacyMode);

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
        }
    }
}
