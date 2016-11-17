using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    [LocalizedModel]
    public class ViewModelWithDuplicateSubModels
    {
        public SubModel SubModelPorperty1 { get; set; }
        public SubModel SubModelPorperty2 { get; set; }
    }

    [LocalizedModel]
    public class SubModel
    {
        [StringLength(5)]
        public string MyProperty { get; set; }
    }

    public class ReusedViewModelTests
    {
        [Fact]
        public void Test1()
        {
            var sut = new TypeDiscoveryHelper();
            var resources = sut.ScanResources(typeof(ViewModelWithDuplicateSubModels));

            Assert.NotNull(resources);

            var count = resources.Count(r => r.Key == "DbLocalizationProvider.Tests.SubModel.MyProperty-StringLength");

            Assert.Equal(1, count);
        }
    }
}
