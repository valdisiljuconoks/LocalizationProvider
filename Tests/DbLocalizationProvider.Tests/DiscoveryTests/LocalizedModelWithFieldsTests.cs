using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class LocalizedModelWithFieldsTests
    {
        [Fact]
        public void DiscoverClassField_WithDefaultValue()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredResources = sut.ScanResources(typeof(LocalizedResourceWithFields));

            // check return
            Assert.NotEmpty(discoveredResources);

            // check discovered translation
            Assert.Equal("sample value", discoveredResources.First().Translation);

            // check generated key from expression
            Assert.Equal("DbLocalizationProvider.Tests.DiscoveryTests.LocalizedResourceWithFields.ThisisField",
                         ExpressionHelper.GetFullMemberName(() => LocalizedResourceWithFields.ThisisField));

            // check discovered resource cache
        }
    }
}
