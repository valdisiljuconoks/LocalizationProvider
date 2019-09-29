using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources
{
    public class ComplexNestedResourceTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public ComplexNestedResourceTests()
        {
            _sut = new TypeDiscoveryHelper();
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
            var key = ExpressionHelper.GetFullMemberName(() => ResourcesWithKeyAndComplexProperties.NestedProperty.SomeProperty);

            Assert.Equal("Prefix.NestedProperty.SomeProperty", key);
        }
    }
}
