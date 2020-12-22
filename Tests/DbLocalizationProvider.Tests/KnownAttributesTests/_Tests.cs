using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    public class CustomAttributeScannerTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public CustomAttributeScannerTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            ConfigurationContext.Current.CustomAttributes.Add<HelpTextAttribute>();
            ConfigurationContext.Current.CustomAttributes.Add<FancyHelpTextAttribute>();
            ConfigurationContext.Current.CustomAttributes.Add<AttributeWithDefaultTranslationAttribute>();

            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state)
            });

        }

        [Fact]
        public void ModelWith2ChildModelAsProperties_ReturnsDuplicates()
        {
            var types = new[]
            {
                typeof(ModelWithTwoChildModelPropertiesCustomAttributes),
                typeof(AnotherModelWithTwoChildModelPropertiesCustomAttributes)
            };

            var resources = types.SelectMany(t => _sut.ScanResources(t)).DistinctBy(r => r.Key);
            var containsDuplicates = resources.GroupBy(r => r.Key).Any(g => g.Count() > 1);

            Assert.False(containsDuplicates);
        }

        [Fact]
        public void ModelWithCustomAttribute_DiscoversResource_PropertyName_As_Translation()
        {
            var resources = _sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal("UserName-HelpText", helpTextResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void ModelWithSingleCustomAttribute_DiscoversBothResources()
        {
            var resources = _sut.ScanResources(typeof(ModelWithSingleCustomAttribute));

            Assert.Equal(2, resources.Count());
        }

        [Fact]
        public void ModelWithDuplicateCustomAttribute_DoesNotThrowException()
        {
            var resources = _sut.ScanResources(typeof(ModelWithCustomAttributesDuplicates));

            Assert.NotNull(resources);
        }

        [Fact]
        public void CanSpecifyDefaultTranslation_UsingToStringOfAttribute()
        {
            var resources = _sut.ScanResources(typeof(ModelWithCustomAttributeWithDefaultTranslation));

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

