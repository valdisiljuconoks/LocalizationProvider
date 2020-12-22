using System.Collections.Generic;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.GenericModels
{
    public class GenericModelTests
    {
        public GenericModelTests()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state),
                new LocalizedEnumTypeScanner(keyBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state)
            });

            ConfigurationContext.Current.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
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
            var key = new ExpressionHelper(new ResourceKeyBuilder(new ScanState())).GetFullMemberName(() => model.BaseProperty);

            Assert.Equal("DbLocalizationProvider.Tests.GenericModels.OpenGenericBase`1.BaseProperty", key);
        }
    }
}
