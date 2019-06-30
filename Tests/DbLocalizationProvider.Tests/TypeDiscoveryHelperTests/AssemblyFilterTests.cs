using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.TypeDiscoveryHelperTests
{
    public class AssemblyFilterTests
    {
        [Fact]
        public void SpecficAssemblyFilter_ShouldIncludeInternal()
        {
            var assemblies = TypeDiscoveryHelper.GetAssemblies(a => a.FullName.StartsWith("NonExisting"), false);

            Assert.NotEmpty(assemblies);
        }

        [Fact]
        public void SpecficAssemblyFilter_IncludesProviderAssemblies_NoDuplicates()
        {
            var assemblies = TypeDiscoveryHelper.GetAssemblies(a => a.FullName.StartsWith("DbLocalizationProvider"), false);

            Assert.NotEmpty(assemblies);
            Assert.NotNull(assemblies.First(a => a.GetName().Name == "DbLocalizationProvider"));
        }
    }
}
