using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace DbLocalizationProvider.AdminUI.Tests
{
    public class ResourceTreeBuilderTests
    {
        [Fact]
        public void MultipleResources_HavingSimilarKeyFragment_RegisteredUnderCorrectParent()
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>
                        {
                            new ResourceListItem("MyNamespace.MyProject.MyResource",
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem("MyNamespace.MyProject.MyResource",
                                                                                             "sample translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false),
                            new ResourceListItem("MyNamespace.MyProject.AnotherResource",
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem("MyNamespace.MyProject.AnotherResource",
                                                                                             "another translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false),

                            new ResourceListItem("OtherNamespace.MyProject.OtherResource",
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem("OtherNamespace.MyProject.OtherResource",
                                                                                             "other translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false)
                        };

            var result = sut.BuildTree(model).ToList();

            Assert.NotNull(result);
            Assert.Equal(7, result.Count);
        }

        [Fact]
        public void SingleResource_SplitIntoKeyFragments_AllRegistered()
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>
                        {
                            new ResourceListItem("MyNamespace.MyProject.MyResource",
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem("MyNamespace.MyProject.MyResource",
                                                                                             "sample translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false)
                        };

            var result = sut.BuildTree(model).ToList();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.False(result.First().IsLeaf);
            Assert.False(result.Skip(1).First().IsLeaf);
            Assert.True(result.Skip(2).First().IsLeaf);
        }

        [Fact]
        public void TwoResources_UnderSingleRootKey_BothRegistered()
        {
            var sut = new ResourceTreeBuilder();
            var model = new List<ResourceListItem>
                        {
                            new ResourceListItem("MyNamespace.MyProject.MyResource",
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem("MyNamespace.MyProject.MyResource",
                                                                                             "sample translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false),
                            new ResourceListItem("MyNamespace.MyProject.AnotherResource",
                                                 new List<ResourceItem>(new[]
                                                                        {
                                                                            new ResourceItem("MyNamespace.MyProject.AnotherResource",
                                                                                             "another translation",
                                                                                             new CultureInfo("en"))
                                                                        }),
                                                 true,
                                                 false)
                        };

            var result = sut.BuildTree(model).ToList();

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
        }
    }
}
