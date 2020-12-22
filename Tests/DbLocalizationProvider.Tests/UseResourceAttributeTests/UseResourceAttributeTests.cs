using System.Collections.Generic;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.UseResourceAttributeTests
{
    public class UseResourceAttributeTests
    {
        private readonly TypeDiscoveryHelper _sut;
        private readonly ExpressionHelper _expressionHelper;

        public UseResourceAttributeTests()
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

            _expressionHelper = new ExpressionHelper(keyBuilder);
        }

        [Fact]
        public void UseResourceAttribute_NoResourceRegistered()
        {
            var results = _sut.ScanResources(typeof(ModelWithOtherResourceUsage));

            Assert.Empty(results);
        }

        [Fact]
        public void UseResourceAttribute_NoResourceRegistered_ResolvedTargetResourceKey()
        {
            var m = new ModelWithOtherResourceUsage();

            _sut.ScanResources(typeof(ModelWithOtherResourceUsage));

            var resultKey = _expressionHelper.GetFullMemberName(() => m.SomeProperty);

            Assert.Equal("DbLocalizationProvider.Tests.UseResourceAttributeTests.CommonResources.CommonProp", resultKey);
        }
    }
}
