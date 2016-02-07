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
            var keyModel = new KeyModel();
            const string modelNameFragment = "DbLocalizationProvider.Tests.KeyModel";

            Assert.Equal($"{modelNameFragment}.SampleProperty", GetMemberFullName(() => keyModel.SampleProperty));
            Assert.Equal($"{modelNameFragment}.SubKeyProperty.AnotherProperty", GetMemberFullName(() => keyModel.SubKeyProperty.AnotherProperty));
            Assert.Equal($"{modelNameFragment}.SubKeyProperty.EvenMoreComplex.Amount", GetMemberFullName(() => keyModel.SubKeyProperty.EvenMoreComplex.Amount));
            Assert.Equal($"{modelNameFragment}.ThisIsConstant", GetMemberFullName(() => KeyModel.ThisIsConstant));
        }

        private static object GetMemberFullName(Expression<Func<object>> memberSelector)
        {
            return ExpressionHelper.GetFullMemberName(memberSelector);
        }
    }
}
