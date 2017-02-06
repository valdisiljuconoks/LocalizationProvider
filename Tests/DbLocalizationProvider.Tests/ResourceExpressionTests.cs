using DbLocalizationProvider.Internal;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ResourceExpressionTests
    {
        [Fact]
        public void Test_PropertyLocalization()
        {
            var keyModel = new ResourceKeys();
            const string modelNameFragment = "DbLocalizationProvider.Tests.ResourceKeys";

            Assert.Equal($"{modelNameFragment}.SampleResource", ExpressionHelper.GetFullMemberName(() => keyModel.SampleResource));
            Assert.Equal($"{modelNameFragment}.SubResource.AnotherResource", ExpressionHelper.GetFullMemberName(() => ResourceKeys.SubResource.AnotherResource));
            Assert.Equal($"{modelNameFragment}.SubResource.EvenMoreComplexResource.Amount",
                         ExpressionHelper.GetFullMemberName(() => ResourceKeys.SubResource.EvenMoreComplexResource.Amount));
            Assert.Equal($"{modelNameFragment}.ThisIsConstant", ExpressionHelper.GetFullMemberName(() => ResourceKeys.ThisIsConstant));
        }
    }
}
