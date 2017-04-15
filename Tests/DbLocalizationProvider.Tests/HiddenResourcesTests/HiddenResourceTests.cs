using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.HiddenResourcesTests
{
    public class HiddenResourceTests
    {
        [Fact]
        public void DiscoverHiddenEnumProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var result = sut.ScanResources(typeof(SomeEnumWithHiddenResources));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.False(firstResource.IsHidden);

            var middleResource = result.First(r => r.Key == "DbLocalizationProvider.Tests.HiddenResourcesTests.SomeEnumWithHiddenResources.Some");
            Assert.True(middleResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenEnumProperties_WithHiddenAttributeOnClassProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var result = sut.ScanResources(typeof(SomeEnumWithAllHiddenResources));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);

            var middleResource = result.First(r => r.Key == "DbLocalizationProvider.Tests.HiddenResourcesTests.SomeEnumWithAllHiddenResources.Some");
            Assert.True(middleResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenModelProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var result = sut.ScanResources(typeof(SomeModelWithHiddenProperty));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenModelProperties_WithHiddenAttributeOnClassProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var result = sut.ScanResources(typeof(SomeModelWithHiddenPropertyOnClassLevel));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenResourceProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var result = sut.ScanResources(typeof(SomeResourcesWithHiddenProperties));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);

            var secondResource = result.First(r => r.Key == "DbLocalizationProvider.Tests.HiddenResourcesTests.SomeResourcesWithHiddenProperties.AnotherProperty");
            Assert.False(secondResource.IsHidden);
        }

        [Fact]
        public void DiscoverHiddenResources_WithHiddenAttributeOnClassProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var result = sut.ScanResources(typeof(SomeResourcesWithHiddenOnClassLevel));

            Assert.NotEmpty(result);
            var firstResource = result.First();
            Assert.True(firstResource.IsHidden);
        }
    }
}
