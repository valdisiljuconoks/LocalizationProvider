using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.TypeDiscoveryHelperTests
{
    public class AssemblyFilterTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public AssemblyFilterTests()
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
        }

        [Fact]
        public void SpecificAssemblyFilter_ShouldIncludeInternal()
        {
            var assemblies = _sut.GetAssemblies(a => a.FullName.StartsWith("NonExisting"), false);

            Assert.NotEmpty(assemblies);
        }

        [Fact]
        public void SpecificAssemblyFilter_IncludesProviderAssemblies_NoDuplicates()
        {
            var assemblies = _sut.GetAssemblies(a => a.FullName.StartsWith("DbLocalizationProvider"), false);

            Assert.NotEmpty(assemblies);
            Assert.NotNull(assemblies.First(a => a.GetName().Name == "DbLocalizationProvider"));
        }
    }
}
