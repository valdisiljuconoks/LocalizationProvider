using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class ResourceExpressionTests
{
    [Fact]
    public void Test_PropertyLocalization()
    {
        var wrapper = new OptionsWrapper<ConfigurationContext>(new ConfigurationContext());
        var expressionHelper = new ExpressionHelper(new ResourceKeyBuilder(new ScanState(), wrapper));

        var keyModel = new ResourceKeys();
        const string modelNameFragment = "DbLocalizationProvider.Tests.ResourceKeys";

        Assert.Equal($"{modelNameFragment}.SampleResource", expressionHelper.GetFullMemberName(() => keyModel.SampleResource));
        Assert.Equal($"{modelNameFragment}.SubResource.AnotherResource",
                     expressionHelper.GetFullMemberName(() => ResourceKeys.SubResource.AnotherResource));
        Assert.Equal($"{modelNameFragment}.SubResource.EvenMoreComplexResource.Amount",
                     expressionHelper.GetFullMemberName(() => ResourceKeys.SubResource.EvenMoreComplexResource.Amount));
        Assert.Equal($"{modelNameFragment}.ThisIsConstant",
                     expressionHelper.GetFullMemberName(() => ResourceKeys.ThisIsConstant));
    }
}
