using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ViewModelWithInheritanceTests
    {
        [Fact]
        public void NotInheritedModel_ContainsOnlyDeclaredProperties()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(SampleViewModelWithBase)).ToList();
            var keys = properties.Select(p => p.Key).ToList();
            var stringLengthResource = properties.FirstOrDefault(r => r.Key == "DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-StringLength");

            Assert.Contains("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description", keys);
            Assert.NotNull(stringLengthResource);
            Assert.Contains("StringLength", stringLengthResource.Translation);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty", keys);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty-Required", keys);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description-Required", keys);
        }
    }
}
