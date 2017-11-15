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
        }

        [Fact]
        public void ModelWith2ChildModelAsProperties_ReturnsDuplicates()
        {
            ConfigurationContext.Current.CustomAttributes = new[]
            {
                new CustomAttributeDescriptor(typeof(HelpTextAttribute), false),
                new CustomAttributeDescriptor(typeof(FancyHelpTextAttribute), false)
            };

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
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(HelpTextAttribute)) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal("UserName-HelpText", helpTextResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void ModelWithCustomAttribute_NullTranslation_DiscoversResource()
        {
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(HelpTextAttribute), false) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal(string.Empty, helpTextResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void ModelWithDuplicateCustomAttribute_DoesNotThrowException()
        {
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(FancyHelpTextAttribute)) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributesDuplicates));

            Assert.NotNull(resources);
        }

        [Fact]
        public void CanSpecifyDefaultTranslation_UsingToStringOfAttribute()
        {
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(AttributeWithDefaultTranslationAttribute)) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributeWithDefaultTranslation));

            Assert.NotNull(resources);
        }

        [Fact]
        public void SpecifyCustomAttributes_TargetIsNotAttribute_Exception()
        {
            Assert.Throws<ArgumentException>(() => ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(CustomAttributeScannerTests)) });
        }
    }
}
