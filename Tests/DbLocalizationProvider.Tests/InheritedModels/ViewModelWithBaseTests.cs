using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    public class ViewModelWithBaseTests
    {
        [Fact]
        public void BaseProperty_HasChildClassResourceKey()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(SampleViewModelWithBase), contextAwareScanning: false)
                                                .Select(p => p.Key)
                                                .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.SampleViewModelWithBase.BaseProperty", properties);
            Assert.Contains("DbLocalizationProvider.Tests.SampleViewModelWithBase.BaseProperty-Required", properties);
        }
    }
}