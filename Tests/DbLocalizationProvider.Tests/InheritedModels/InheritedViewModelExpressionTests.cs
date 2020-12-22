using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
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
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state)
            });

            var expressionHelper = new ExpressionHelper(keyBuilder);

            var properties = new[] { typeof(SampleViewModelWithBaseNotInherit), typeof(BaseLocalizedViewModel) }
                .Select(t => sut.ScanResources(t))
                .ToList();

            var childModel = new SampleViewModelWithBaseNotInherit();
            var basePropertyKey = expressionHelper.GetFullMemberName(() => childModel.BaseProperty);

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty", basePropertyKey);
        }
    }
}
