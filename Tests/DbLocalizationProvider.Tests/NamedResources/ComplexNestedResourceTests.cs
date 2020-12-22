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
            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state)
            });

            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
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
