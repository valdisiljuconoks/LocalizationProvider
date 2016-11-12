using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ScannerTests
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
    }
}
