using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class DataAnnotationsTests
    {
        public DataAnnotationsTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>()
                .SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void ChildClassTypeAttributeUsage_ShouldRegisterResource()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(ViewModelWithInheritedDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Equal(2, properties.Count());
        }

        [Fact]
        public void DirectDataTypeAttributeUsage_ShouldNotRegisterResource()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(ViewModelWithSomeDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Equal(1, properties.Count());
        }
    }
}
