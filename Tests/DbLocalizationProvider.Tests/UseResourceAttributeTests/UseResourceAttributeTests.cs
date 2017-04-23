using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.UseResourceAttributeTests
{
    public class UseResourceAttributeTests
    {
        [Fact]
        public void UseResourceAttribute_NoResourceRegistered()
        {
            var sut = new TypeDiscoveryHelper();

            var results = sut.ScanResources(typeof(ModelWithOtherResourceUsage));

            Assert.Empty(results);
        }

        [Fact]
        public void UseResourceAttribute_NoResourceRegistered_ResolvedTargetResourceKey()
        {
            var sut = new TypeDiscoveryHelper();
            var m = new ModelWithOtherResourceUsage();

            sut.ScanResources(typeof(ModelWithOtherResourceUsage));

            var resultKey = ExpressionHelper.GetFullMemberName(() => m.SomeProperty);

            Assert.Equal("DbLocalizationProvider.Tests.UseResourceAttributeTests.CommonResources.CommonProp", resultKey);
        }
    }
}
