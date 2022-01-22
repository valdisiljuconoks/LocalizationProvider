using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ResourceExpressionTests
    {
        [Fact]
        public void Test_PropertyLocalization()
        {
            var expressionHelper = new ExpressionHelper(new ResourceKeyBuilder(new ScanState()));

            var keyModel = new ResourceKeys();
            const string modelNameFragment = "DbLocalizationProvider.Tests.ResourceKeys";

            Assert.Equal($"{modelNameFragment}.SampleResource", expressionHelper.GetFullMemberName(() => keyModel.SampleResource));
            Assert.Equal($"{modelNameFragment}.SubResource.AnotherResource", expressionHelper.GetFullMemberName(() => ResourceKeys.SubResource.AnotherResource));
            Assert.Equal($"{modelNameFragment}.SubResource.EvenMoreComplexResource.Amount",
                         expressionHelper.GetFullMemberName(() => ResourceKeys.SubResource.EvenMoreComplexResource.Amount));
            Assert.Equal($"{modelNameFragment}.ThisIsConstant", expressionHelper.GetFullMemberName(() => ResourceKeys.ThisIsConstant));
        }
    }
}
