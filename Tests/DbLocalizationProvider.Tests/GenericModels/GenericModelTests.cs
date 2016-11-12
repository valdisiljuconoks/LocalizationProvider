using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.GenericModels
{
    public class GenericModelTests
    {
        public GenericModelTests()
        {
            _sut = new TypeDiscoveryHelper();
        }

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public void TestGenericProperty()
        {
            var properties = _sut.ScanResources(typeof(OpenGenericModel<>));

            Assert.NotEmpty(properties);
        }

        [Fact]
        public void TestGenericProperty_FromChildClass()
        {
            var properties = _sut.ScanResources(typeof(ClosedGenericModel));

            Assert.NotEmpty(properties);
        }
    }
}
