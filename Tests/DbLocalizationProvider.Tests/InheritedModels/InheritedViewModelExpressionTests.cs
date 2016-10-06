using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    public class InheritedViewModelExpressionTests
    {
        [Fact]
        public void Test()
        {
            var properties =
                new[] { typeof(SampleViewModelWithBaseNotInherit), typeof(BaseLocalizedViewModel) }
                    .Select(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false))
                    .ToList();

            var childModel = new SampleViewModelWithBaseNotInherit();
            var basePropertyKey = ExpressionHelper.GetFullMemberName(() => childModel.BaseProperty);

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty", basePropertyKey);
        }
    }
}
