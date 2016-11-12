using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources
{
    public class NamedResourcesTests
    {
        public NamedResourcesTests()
        {
            _sut = new TypeDiscoveryHelper();
        }

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public void DuplicateAttributes_DiffProperties_SameKey_ThrowsException()
        {
            var model = new[] { typeof(BadResourceWithDuplicateKeysWithinClass) };
            Assert.Throws<DuplicateResourceKeyException>(() => model.SelectMany(t => _sut.ScanResources(t)).ToList());
        }

        [Fact]
        public void DuplicateAttributes_SingleProperty_SameKey_ThrowsException()
        {
            var model = new[] { typeof(BadResourceWithDuplicateKeys) };
            Assert.Throws<DuplicateResourceKeyException>(() => model.SelectMany(t => _sut.ScanResources(t)).ToList());
        }

        [Fact]
        public void ExpressionTest_WithNamedResources_NoPrefix_ReturnsResourceKey()
        {
            var result = ExpressionHelper.GetFullMemberName(() => ResourcesWithNamedKeys.PageHeader);

            Assert.Equal("/this/is/xpath/to/resource", result);
        }

        [Fact]
        public void ExpressionTest_WithNamedResources_WithPrefix_ReturnsResourceKey()
        {
            var result = ExpressionHelper.GetFullMemberName(() => ResourcesWithNamedKeysWithPrefix.PageHeader);

            Assert.Equal("/this/is/root/resource/and/this/is/header", result);
        }

        [Fact]
        public void MultipleAttributesForSingleProperty_NoPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeys)}");

            var properties = model.SelectMany(t => _sut.ScanResources(t)).ToList();

            var namedResource = properties.FirstOrDefault(p => p.Key == "/this/is/xpath/to/resource");

            Assert.NotNull(namedResource);
            Assert.Equal("This is header", namedResource.Translation);
        }

        [Fact]
        public void MultipleAttributesForSingleProperty_WithPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeysWithPrefix)}");

            var properties = model.SelectMany(t => _sut.ScanResources(t)).ToList();

            var namedResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/this/is/header");

            Assert.NotNull(namedResource);
            Assert.Equal("This is header", namedResource.Translation);

            var firstResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/1stresource");

            Assert.NotNull(firstResource);
            Assert.Equal("Value in attribute", firstResource.Translation);

            var secondResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/2ndresource");

            Assert.NotNull(secondResource);
            Assert.Equal("This is property value", secondResource.Translation);
        }
    }
}
