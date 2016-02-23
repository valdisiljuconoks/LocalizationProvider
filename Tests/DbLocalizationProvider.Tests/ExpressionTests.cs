using System;
using System.Linq.Expressions;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ExpressionTests
    {
        [Fact]
        public void Test_PropertyLocalization()
        {
            var keyModel = new ResourceKeys();
            const string modelNameFragment = "DbLocalizationProvider.Tests.ResourceKeys";

            Assert.Equal($"{modelNameFragment}.SampleResource", GetMemberFullName(() => keyModel.SampleResource));
            Assert.Equal($"{modelNameFragment}.SubResource.AnotherResource", GetMemberFullName(() => ResourceKeys.SubResource.AnotherResource));
            Assert.Equal($"{modelNameFragment}.SubResource.EvenMoreComplexResource.Amount", GetMemberFullName(() => ResourceKeys.SubResource.EvenMoreComplexResource.Amount));
            Assert.Equal($"{modelNameFragment}.ThisIsConstant", GetMemberFullName(() => ResourceKeys.ThisIsConstant));
        }

        private static object GetMemberFullName(Expression<Func<object>> memberSelector)
        {
            return ExpressionHelper.GetFullMemberName(memberSelector);
        }
    }
}
