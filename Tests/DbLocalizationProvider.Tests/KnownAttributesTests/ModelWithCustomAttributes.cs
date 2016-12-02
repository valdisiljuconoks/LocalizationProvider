using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    public class HelpTextAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FancyHelpTextAttribute : Attribute { }

    [LocalizedModel]
    public class ModelWithCustomAttributes
    {
        [Display(Name = "User name")]
        [Required]
        [StringLength(5)]
        [HelpText]
        public string UserName { get; set; }
    }

    [LocalizedModel]
    public class ModelWithCustomAttributesDuplicates
    {
        [FancyHelpText]
        [FancyHelpText]
        public string UserName { get; set; }
    }

    public class CustomAttributeScannerTests
    {
        [Fact]
        public void ModelWithDuplicateCustomAttribute_DoesNotThrowException()
        {
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(FancyHelpTextAttribute)) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributesDuplicates));

            Assert.NotNull(resources);
        }

        [Fact]
        public void ModelWithCustomAttribute_DiscoversResource_PropertyName_As_Translation()
        {
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(HelpTextAttribute)) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal("UserName-HelpText", helpTextResource.Translation);
        }

        [Fact]
        public void ModelWithCustomAttribute_NullTranslation_DiscoversResource()
        {
            ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(HelpTextAttribute), false) };
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal(string.Empty, helpTextResource.Translation);
        }

        [Fact]
        public void SpecifyCustomAttributes_InvalidType_Exception()
        {
            Assert.Throws<ArgumentException>(() => ConfigurationContext.Current.CustomAttributes = new[] { new CustomAttributeDescriptor(typeof(CustomAttributeScannerTests)) });
        }
    }
}
