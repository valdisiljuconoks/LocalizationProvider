using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace DbLocalizationProvider.AdminUI.Tests
{
    public class ResourceTreeSorterTests
    {
        [Fact]
        public void TwoResources_UnderSingleRootKey_VerifyOrder()
        {
            var sut = new ResourceTreeSorter();
            var builder = new ResourceTreeBuilder();

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

            var result = sut.Sort(builder.BuildTree(model)).ToList();

            Assert.NotNull(result);
            Assert.Equal(4, result.Count);

            Assert.Equal("MyNamespace", result.First().KeyFragment);
            Assert.Equal("MyResource", result.Skip(3).First().KeyFragment);
        }

        [Fact]
        public void MultipleResources_HavingSimilarKeyFragment_RegisteredUnderCorrectParent_VerifyOrder()
        {
            var sut = new ResourceTreeSorter();
            var builder = new ResourceTreeBuilder();
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

            var result = sut.Sort(builder.BuildTree(model)).ToList();

            Assert.NotNull(result);
            Assert.Equal(7, result.Count);
            Assert.Equal(5, result.Single(r => r.KeyFragment == "OtherResource").ParentId);
        }
    }
}
