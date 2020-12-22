using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests
{
    public class TypeScannerTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public TypeScannerTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state)
            });
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var sut = new LocalizedResourceTypeScanner(keyBuilder, new OldResourceKeyBuilder(keyBuilder), state);

            var result = sut.GetResources(typeof(PageResources), null);

            Assert.True(result.Any());
            Assert.Equal("Header", result.First().Translations.DefaultTranslation());
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName_ViaTypeDiscoveryHelper()
        {
            var result = _sut.ScanResources(typeof(CommonResources.DialogResources)).ToList();

            Assert.True(result.Any());
            Assert.Equal("YesButton", result.First(r => r.PropertyName == "YesButton").Translations.DefaultTranslation());
            Assert.Equal("NullProperty", result.First(r => r.PropertyName == "NullProperty").Translations.DefaultTranslation());
        }

        [Fact]
        public void ScanStackOverflowResource_WithPropertyReturningBaseDeclaringType()
        {
            Assert.Throws<RecursiveResourceReferenceException>(() =>
                _sut.ScanResources(typeof(BadRecursiveResource_BaseDeclaringType)));
        }

        [Fact]
        public void ScanStackOverflowResource_WithPropertyReturningSameDeclaringType()
        {
            Assert.Throws<RecursiveResourceReferenceException>(() =>
                _sut.ScanResources(typeof(BadRecursiveResource_SameDeclaringType)));
        }

        [Fact]
        public void ViewModelType_ShouldSelectModelScanner()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var sut = new LocalizedModelTypeScanner(keyBuilder, new OldResourceKeyBuilder(keyBuilder), state);

            var result = sut.ShouldScan(typeof(SampleViewModel));

            Assert.True(result);
        }
    }
}
