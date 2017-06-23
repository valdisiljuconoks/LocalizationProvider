using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    public class InheritedViewModelExpressionTests
    {
        public InheritedViewModelExpressionTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        }

        [Fact]
        public void Test()
        {
            var sut = new TypeDiscoveryHelper();

            var properties = new[] { typeof(SampleViewModelWithBaseNotInherit), typeof(BaseLocalizedViewModel) }
                .Select(t => sut.ScanResources(t))
                .ToList();

            var childModel = new SampleViewModelWithBaseNotInherit();
            var basePropertyKey = ExpressionHelper.GetFullMemberName(() => childModel.BaseProperty);

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty", basePropertyKey);
        }
    }
}
