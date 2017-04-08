using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class TypeScannerTests
    {
        [Fact]
        public void ViewModelType_ShouldSelectModelScanner()
        {
            var sut = new LocalizedModelTypeScanner();

            var result = sut.ShouldScan(typeof(SampleViewModel));

            Assert.True(result);
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName()
        {
            var sut = new LocalizedResourceTypeScanner();

            var result = sut.GetResources(typeof(PageResources), null);

            Assert.True(result.Any());
            Assert.Equal("Header", result.First().Translation);
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName_ViaTypeDiscoveryHelper()
        {
            var sut = new TypeDiscoveryHelper();

            var result = sut.ScanResources(typeof(CommonResources.DialogResources)).ToList();

            Assert.True(result.Any());
            Assert.Equal("YesButton", result.First(r => r.PropertyName == "YesButton").Translation);
            Assert.Equal("NullProperty", result.First(r => r.PropertyName == "NullProperty").Translation);
        }
    }
}
