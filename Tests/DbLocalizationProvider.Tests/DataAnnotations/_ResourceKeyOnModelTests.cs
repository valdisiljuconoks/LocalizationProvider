using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public void Test1()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(ModelWithDataAnnotationsAndResourceKey));

            Assert.NotEmpty(properties);
            Assert.Equal(2, properties.Count());
        }
    }

    [LocalizedModel]
    public class ModelWithDataAnnotationsAndResourceKey
    {
        [ResourceKey("the-key")]
        [Display(Name = "Something")]
        [Required]
        public string UserName { get; set; }
    }
}
