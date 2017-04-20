using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class DataValidationAttributesTests
    {
        [Fact]
        public void DataTypeAttributeTest_ShouldNotRegisterResource()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(ViewModelWithSomeDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Equal(2, properties.Count());
        }
    }
}
