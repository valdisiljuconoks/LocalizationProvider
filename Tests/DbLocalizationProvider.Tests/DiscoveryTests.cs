using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class DiscoveryTests
    {
        [Fact]
        public void SingleLevel_ScalarProperties_NoAttribute()
        {
            var types = TypeDiscoveryHelper.GetTypesOfInterface<ILocalizedModel>().ToList();

            Assert.NotEmpty(types);

            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type);

            var staticField = properties.First(p => p.Item2 == "DbLocalizationProvider.Tests.KeyModel.ThisIsConstant");

            Assert.True(TypeDiscoveryHelper.IsStaticStringProperty(staticField.Item1));
        }

        [Fact]
        public void NestedObject_ScalarProperties_NoAttribute()
        {
            var types = TypeDiscoveryHelper.GetTypesOfInterface<ILocalizedModel>().ToList();
            var type = types.First();
            var properties = TypeDiscoveryHelper.GetAllProperties(type).ToList();

            Assert.Contains("DbLocalizationProvider.Tests.KeyModel.SubKeyProperty.AnotherProperty", properties.Select(k => k.Item2));
            Assert.Contains("DbLocalizationProvider.Tests.KeyModel.SubKeyProperty.EvenMoreComplex.Amount", properties.Select(k => k.Item2));
        }
    }
}
