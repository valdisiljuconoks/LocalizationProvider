using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    [LocalizedModel]
    public class LocalizedModelWithFields
    {
        public string ThisisField = "sample value";
    }

    public class LocalizedModelWithFieldsTests
    {
        [Fact]
        public void DiscoverClassField_WithDefaultValue()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredResources = sut.ScanResources(typeof(LocalizedModelWithFields));

            // check return
            //Assert.NotEmpty(discoveredResources);

            // check discovered resource cache

        }
    }
}