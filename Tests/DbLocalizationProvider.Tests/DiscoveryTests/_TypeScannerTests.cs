using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class TypeScannerTests
    {
        public TypeScannerTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName()
        {
            var sut = new LocalizedResourceTypeScanner();

            var result = sut.GetResources(typeof(PageResources), null);

            Assert.True(result.Any());
            Assert.Equal("Header", result.First().Translations.DefaultTranslation());
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName_ViaTypeDiscoveryHelper()
        {
            var sut = new TypeDiscoveryHelper();

            var result = sut.ScanResources(typeof(CommonResources.DialogResources)).ToList();

            Assert.True(result.Any());
            Assert.Equal("YesButton", result.First(r => r.PropertyName == "YesButton").Translations.DefaultTranslation());
            Assert.Equal("NullProperty", result.First(r => r.PropertyName == "NullProperty").Translations.DefaultTranslation());
        }

        [Fact]
        public void ScanStackOverflowResource_WithPropertyReturningBaseDeclaringType()
        {
            var sut = new TypeDiscoveryHelper();
            Assert.Throws<RecursiveResourceReferenceException>(() =>
                sut.ScanResources(typeof(BadRecursiveResource_BaseDeclaringType)));
        }

        [Fact]
        public void ScanStackOverflowResource_WithPropertyReturningSameDeclaringType()
        {
            var sut = new TypeDiscoveryHelper();

            Assert.Throws<RecursiveResourceReferenceException>(() =>
                sut.ScanResources(typeof(BadRecursiveResource_SameDeclaringType)));
        }

        [Fact]
        public void ViewModelType_ShouldSelectModelScanner()
        {
            var sut = new LocalizedModelTypeScanner();

            var result = sut.ShouldScan(typeof(SampleViewModel));

            Assert.True(result);
        }
    }
}
