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
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);
        }

        [Fact]
        public void Resource_WithJustStaticGetSet_TranslationShouldBePropertyName()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);
            var sut = new LocalizedResourceTypeScanner(keyBuilder, new OldResourceKeyBuilder(keyBuilder), state, ctx, translationBuilder);

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
        public void ViewModelType_ShouldSelectModelScanner()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);
            var sut = new LocalizedModelTypeScanner(keyBuilder, new OldResourceKeyBuilder(keyBuilder), state, ctx, translationBuilder);

            var result = sut.ShouldScan(typeof(SampleViewModel));

            Assert.True(result);
        }
    }
}
