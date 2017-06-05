using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.GenericModels
{
    public class GenericModelTests
    {
        public GenericModelTests()
        {
            _sut = new TypeDiscoveryHelper();
        }

        private readonly TypeDiscoveryHelper _sut;

        [Fact]
        public void TestGenericProperty()
        {
            var properties = _sut.ScanResources(typeof(OpenGenericModel<>));

            Assert.NotEmpty(properties);
        }

        [Fact]
        public void TestGenericProperty_FromChildClass()
        {
            var properties = _sut.ScanResources(typeof(ClosedGenericModel));

            Assert.NotEmpty(properties);
        }

        [Fact]
        public void TestGenericProperty_FromChildClass_WithNoInherit()
        {
            var properties1 = _sut.ScanResources(typeof(OpenGenericBase<>));
            var properties2 = _sut.ScanResources(typeof(CloseGenericNoInherit));

            Assert.NotEmpty(properties1);
            Assert.NotEmpty(properties2);

            var model = new CloseGenericNoInherit();
            var key = ExpressionHelper.GetFullMemberName(() => model.BaseProperty);

            Assert.Equal("DbLocalizationProvider.Tests.GenericModels.OpenGenericBase`1.BaseProperty", key);
        }
    }
}
