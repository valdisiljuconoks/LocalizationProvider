using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources
{
    public class ComplexNestedResourceTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public ComplexNestedResourceTests()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx);
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
        public void ComplexProperty_OnClassWithKey_PropertyGetsCorrectKey()
        {
            var result = _sut.ScanResources(typeof(ResourcesWithKeyAndComplexProperties));

            Assert.NotEmpty(result);
            Assert.Equal("Prefix.NestedProperty.SomeProperty", result.First().Key);
        }

        [Fact]
        public void ComplexProperty_OnClassWithKey_ExprEvaluatesCorrectKey()
        {
            var key = new ExpressionHelper(new ResourceKeyBuilder(new ScanState())).GetFullMemberName(
                () => ResourcesWithKeyAndComplexProperties.NestedProperty.SomeProperty);

            Assert.Equal("Prefix.NestedProperty.SomeProperty", key);
        }
    }
}
