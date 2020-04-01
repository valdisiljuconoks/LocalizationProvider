using System;
using System.Linq.Expressions;
using FakeItEasy;
using Xunit;

namespace DbLocalizationProvider.Tests.InterfaceTests
{
    public class Tests
    {
        [Fact]
        public void TestInterfaceMock()
        {
            var fake = A.Fake<ILocalizationProvider>();

            Expression<Func<object>> expression = () => ResourceClass.SomeProperty;
            Expression<Func<object>> expression2 = () => ResourceClass.SomeProperty2;
            var comparer = new ExpressionComparer();

            A.CallTo(() => fake.GetString(A<Expression<Func<object>>>.That.Matches(e => comparer.Equals(expression, e))))
             .Returns("[SomeProperty] Value from fake");
            A.CallTo(() => fake.GetString(A<Expression<Func<object>>>.That.Matches(e => comparer.Equals(expression2, e))))
             .Returns("[SomeProperty] Value from fake 2");

            var sut = new SomeServiceWithLocalization(fake);
            var result = sut.GetTranslation();
            var result2 = sut.GetTranslation2();

            Assert.Equal("[SomeProperty] Value from fake", result);
            Assert.Equal("[SomeProperty] Value from fake 2", result2);
        }
    }

    public class ResourceClass
    {
        public static string SomeProperty { get; } = "Some value of the property";
        public static string SomeProperty2 { get; } = "Another translation";
    }

    public class SomeServiceWithLocalization
    {
        private readonly ILocalizationProvider _provider;

        public SomeServiceWithLocalization(ILocalizationProvider provider)
        {
            _provider = provider;
        }

        public string GetTranslation()
        {
            return _provider.GetString(() => ResourceClass.SomeProperty);
        }

        public string GetTranslation2()
        {
            return _provider.GetString(() => ResourceClass.SomeProperty2);
        }
    }
}
