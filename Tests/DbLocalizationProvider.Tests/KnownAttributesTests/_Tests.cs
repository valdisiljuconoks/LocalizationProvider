using System;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    public class CustomAttributeScannerTests
    {
        public CustomAttributeScannerTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            ConfigurationContext.Current.CustomAttributes.Add<HelpTextAttribute>();
            ConfigurationContext.Current.CustomAttributes.Add<FancyHelpTextAttribute>();
            ConfigurationContext.Current.CustomAttributes.Add<AttributeWithDefaultTranslationAttribute>();
        }

        [Fact]
        public void ModelWith2ChildModelAsProperties_ReturnsDuplicates()
        {
            var sut = new TypeDiscoveryHelper();
            var types = new[]
            {
                typeof(ModelWithTwoChildModelPropertiesCustomAttributes),
                typeof(AnotherModelWithTwoChildModelPropertiesCustomAttributes)
            };

            var resources = types.SelectMany(t => sut.ScanResources(t)).DistinctBy(r => r.Key);
            var containsDuplicates = resources.GroupBy(r => r.Key).Any(g => g.Count() > 1);

            Assert.False(containsDuplicates);
        }

        [Fact]
        public void ModelWithCustomAttribute_DiscoversResource_PropertyName_As_Translation()
        {
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal("UserName-HelpText", helpTextResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void ModelWithSingleCustomAttribute_DiscoversBothResources()
        {
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithSingleCustomAttribute));

            Assert.Equal(2, resources.Count());
        }

        [Fact]
        public void ModelWithDuplicateCustomAttribute_DoesNotThrowException()
        {
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributesDuplicates));

            Assert.NotNull(resources);
        }

        [Fact]
        public void CanSpecifyDefaultTranslation_UsingToStringOfAttribute()
        {
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributeWithDefaultTranslation));

            Assert.NotNull(resources);

            var foreignResource = resources.First(r => r.PropertyName == "SomeProperty-WithDefaultTranslation");
            Assert.Equal("This is default translation", foreignResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void SpecifyCustomAttributes_TargetIsNotAttribute_Exception()
        {
            Assert.Throws<ArgumentException>(() =>
                ConfigurationContext.Current.CustomAttributes.Add<CustomAttributeScannerTests>());
        }
    }
}

