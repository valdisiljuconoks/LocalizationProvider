using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.InheritedModels
{
    public class ViewModelWithBaseTests
    {
        [Fact]
        public void BaseProperty_HasChildClassResourceKey()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(SampleViewModelWithBase), contextAwareScanning: false)
                                                .Select(p => p.Key)
                                                .ToList();

            Assert.Contains("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBase.BaseProperty", properties);
            Assert.Contains("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBase.BaseProperty-Required", properties);
        }

        [Fact]
        public void BaseProperty_HasChildClassResourceKey_DoesNotIncludeInheritedProperties()
        {
            var properties = TypeDiscoveryHelper.GetAllProperties(typeof(SampleViewModelWithBaseNotInherit), contextAwareScanning: false)
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
                    .Select(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false))
                    .ToList();

            var childPropertyKey = ResourceKeyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "ChildProperty");
            var basePropertyKey = ResourceKeyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "BaseProperty");
            var requiredBasePropertyKey = ResourceKeyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "BaseProperty", new RequiredAttribute());

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.SampleViewModelWithBaseNotInherit.ChildProperty", childPropertyKey);
            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty", basePropertyKey);
            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.BaseLocalizedViewModel.BaseProperty-Required", requiredBasePropertyKey);
        }

        [Fact]
        public void BuildResourceKey_ForSecondBaseClassProperty_ExcludedFromChild_ShouldReturnBaseTypeContext()
        {
            var properties =
                new[] { typeof(SampleViewModelWithBaseNotInherit), typeof(BaseLocalizedViewModel), typeof(VeryBaseLocalizedViewModel) }
                    .Select(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false))
                    .ToList();

            var veryBasePropertyKey = ResourceKeyBuilder.BuildResourceKey(typeof(SampleViewModelWithBaseNotInherit), "VeryBaseProperty");

            Assert.Equal("DbLocalizationProvider.Tests.InheritedModels.VeryBaseLocalizedViewModel.VeryBaseProperty", veryBasePropertyKey);
        }
    }
}
