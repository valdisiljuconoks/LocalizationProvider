using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ViewModelWithInheritanceTests
    {
        [Fact]
        public void NotInheritedModel_ContainsOnlyDeclaredProperties()
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

            var properties = sut.ScanResources(typeof(SampleViewModelWithBase)).ToList();
            var keys = properties.Select(p => p.Key).ToList();
            var stringLengthResource = properties.FirstOrDefault(r => r.Key == "DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-StringLength");

            Assert.Contains("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description", keys);
            Assert.NotNull(stringLengthResource);
            Assert.Contains("StringLength", stringLengthResource.Translations.DefaultTranslation());
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty", keys);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty-Required", keys);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description-Required", keys);
        }
    }
}
