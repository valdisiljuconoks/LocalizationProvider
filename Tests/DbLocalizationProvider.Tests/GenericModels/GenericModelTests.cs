using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.GenericModels
{
    public class GenericModelTests
    {
        [Fact]
        public void TestGenericProperty()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(OpenGenericModel<>), contextAwareScanning: false);

            Assert.NotEmpty(properties);
        }

        [Fact]
        public void TestGenericProperty_FromChildClass()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(ClosedGenericModel), contextAwareScanning: false);

            Assert.NotEmpty(properties);
        }
    }
}