using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ResourceKeyOnModelTests
    {
        public ResourceKeyOnModelTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>()
                                .SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void ModelWithResourceKeysOnValidationAttributes_GetsCorrectCustomKey()
        {
            var sut = new TypeDiscoveryHelper();
            var container = typeof(ModelWithDataAnnotationsAndResourceKey);
            var properties = sut.ScanResources(container);

            Assert.NotEmpty(properties);
            Assert.Equal(2, properties.Count());
            Assert.NotNull(properties.First(r => r.Key == "the-key"));

            var requiredResource = properties.First(r => r.Key == "the-key-Required");
            Assert.NotNull(requiredResource);
            Assert.Equal("User name is required!", requiredResource.Translations.DefaultTranslation());

            var resourceKey = ResourceKeyBuilder.BuildResourceKey(container, nameof(ModelWithDataAnnotationsAndResourceKey.UserName));
            Assert.Equal("the-key", resourceKey);

            var requiredResourceKey = ResourceKeyBuilder.BuildResourceKey(container, nameof(ModelWithDataAnnotationsAndResourceKey.UserName), new RequiredAttribute());
            Assert.Equal("the-key-Required", requiredResourceKey);
        }

        [Fact]
        public void ModelWithDataValidationAndMoreResourceKeys_ThrowsException()
        {
            var sut = new TypeDiscoveryHelper();

            Assert.Throws<InvalidOperationException>(() =>
                                                         sut.ScanResources(typeof(ModelWithDataAnnotationsAndManyResourceKeys)));
        }
    }

    [LocalizedModel]
    public class ModelWithDataAnnotationsAndResourceKey
    {
        [ResourceKey("the-key")]
        [Display(Name = "Something")]
        [Required(ErrorMessage = "User name is required!")]
        public string UserName { get; set; }
    }

    [LocalizedModel]
    public class ModelWithDataAnnotationsAndManyResourceKeys
    {
        [ResourceKey("the-key")]
        [ResourceKey("the-key2")]
        [Required]
        public string UserName { get; set; }
    }
}
