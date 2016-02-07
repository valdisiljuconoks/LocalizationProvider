using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class DiscoveryTests
    {
        [Fact]
        public void SingleLevel_ScalarProperties_NoAttributes()
        {
            var types = TypeDiscoveryHelper.GetTypesOfInterface<ILocalizedModel>().ToList();

            Assert.NotEmpty(types);

            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type);

            var staticField = properties.First(p => p.Item2 == "DbLocalizationProvider.Tests.KeyModel.ThisIsConstant");

            Assert.True(TypeDiscoveryHelper.IsStaticStringProperty(staticField.Item1));
        }
    }
}
