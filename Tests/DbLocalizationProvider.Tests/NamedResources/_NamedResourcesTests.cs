using System.Linq;
using System.Reflection;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources
{
    public class NamedResourcesTests
    {
        public NamedResourcesTests()
        {
            _sut = new TypeDiscoveryHelper();
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public void DuplicateAttributes_DiffProperties_SameKey_ThrowsException()
        {
            var model = new[] { typeof(BadResourceWithDuplicateKeysWithinClass) };
            Assert.Throws<DuplicateResourceKeyException>(() => model.SelectMany(t => _sut.ScanResources(t)).ToList());
        }

        [Fact]
        public void ClassLevelResourceKeys_Discovers()
        {
            var model = typeof(ResourceWithClassLevelAttribute);
            var result = _sut.ScanResources(model);

            Assert.NotEmpty(result);
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
            Assert.Equal("This is header", namedResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void MultipleAttributesForSingleProperty_WithPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeysWithPrefix)}");

            var properties = model.SelectMany(t => _sut.ScanResources(t)).ToList();

            var namedResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/this/is/header");

            Assert.NotNull(namedResource);
            Assert.Equal("This is header", namedResource.Translations.DefaultTranslation());

            var firstResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/1stresource");

            Assert.NotNull(firstResource);
            Assert.Equal("Value in attribute", firstResource.Translations.DefaultTranslation());

            var secondResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/2ndresource");

            Assert.NotNull(secondResource);
            Assert.Equal("This is property value", secondResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void MultipleAttributesForSingleProperty_WithPrefix_KeyBuilderTest()
        {
            Assert.Throws<AmbiguousMatchException>(() => ResourceKeyBuilder.BuildResourceKey(typeof(ResourcesWithNamedKeysWithPrefix), nameof(ResourcesWithNamedKeysWithPrefix.SomeResource)));
        }
    }
}
