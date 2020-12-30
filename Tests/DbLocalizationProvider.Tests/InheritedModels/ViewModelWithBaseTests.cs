using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    public class ViewModelWithBaseTests
    {
        private readonly TypeDiscoveryHelper _sut;
        private readonly ResourceKeyBuilder _keyBuilder;

        public ViewModelWithBaseTests()
        {
            var state = new ScanState();
            _keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(_keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(_keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(_keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(_keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(_keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);
        }

        [Fact]
        public void BaseProperty_HasChildClassResourceKey()
        {
            var properties = _sut.ScanResources(typeof(SampleViewModelWithBase))
                                 .Select(p => p.Key)
                                 .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBase.BaseProperty", properties);
            Assert.Contains("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBase.BaseProperty-Required", properties);
        }

        [Fact]
        public void BaseProperty_HasChildClassResourceKey_DoesNotIncludeInheritedProperties()
        {
            var properties = _sut.ScanResources(typeof(SampleViewModelWithBaseNotInherit))
                                 .Select(p => p.Key)
                                 .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBaseNotInherit.ChildProperty", properties);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBaseNotInherit.BaseProperty", properties);
        }

        [Fact]
        public void BuildResourceKey_ForBaseClassProperty_ExcludedFromChild_ShouldReturnBaseTypeContext()
        {
            var properties =
                new[] { typeof(SampleViewModelWithBaseNotInherit), typeof(BaseLocalizedViewModel) }
                    .Select(t => _sut.ScanResources(t))
                    .ToList();

            var childPropertyKey = _keyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "ChildProperty");
            var basePropertyKey = _keyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "BaseProperty");
            var requiredBasePropertyKey = _keyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "BaseProperty", new RequiredAttribute());

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBaseNotInherit.ChildProperty", childPropertyKey);
            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty", basePropertyKey);
            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty-Required", requiredBasePropertyKey);
        }

        [Fact]
        public void BuildResourceKey_ForSecondBaseClassProperty_ExcludedFromChild_ShouldReturnBaseTypeContext()
        {
            var properties =
                new[] { typeof(SampleViewModelWithBaseNotInherit), typeof(BaseLocalizedViewModel), typeof(VeryBaseLocalizedViewModel) }
                    .Select(t => _sut.ScanResources(t))
                    .ToList();

            var veryBasePropertyKey = _keyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "VeryBaseProperty");

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.VeryBaseLocalizedViewModel.VeryBaseProperty", veryBasePropertyKey);
        }

        [Fact]
        public void TestOpenGenericRegistration_ClosedGenericLookUp_ShouldFindSame()
        {
            TypeDiscoveryHelper.DiscoveredResourceCache.TryAdd(typeof(BaseOpenViewModel<>).FullName, new List<string> { "Message" });

            var type = new SampleViewModelWithClosedBase();

            var key = _keyBuilder.BuildResourceKey(type.GetType(), "Message");

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseOpenViewModel`1.Message", key);
        }
    }
}
