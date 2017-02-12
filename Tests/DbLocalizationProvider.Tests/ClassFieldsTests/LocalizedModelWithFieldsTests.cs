using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.ClassFieldsTests
{
    public class LocalizedModelWithFieldsTests
    {
        [Fact]
        public void DiscoverClassField_ChildClassWithNoInherit_FieldIsNotInChildClassNamespace()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredModels = new[] { typeof(LocalizedChildModelWithFields), typeof(LocalizedBaseModelWithFields) }
                .SelectMany(t => sut.ScanResources(t)).ToList();

            // check return
            Assert.NotEmpty(discoveredModels);
        }

        [Fact]
        public void DiscoverClassField_OnlyIncluded()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredModels = sut.ScanResources(typeof(LocalizedModelWithOnlyIncludedFields));

            // check return
            Assert.NotEmpty(discoveredModels);

            // check translation
            Assert.Equal("yet other value", discoveredModels.First().Translation);
        }

        [Fact]
        public void DiscoverClassField_WithDefaultValue()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredModels = sut.ScanResources(typeof(LocalizedModelWithFields));

            // check return
            Assert.NotEmpty(discoveredModels);

            // check discovered translation
            Assert.Equal("other value", discoveredModels.First().Translation);

            //// check generated key from expression
            Assert.Equal("DbLocalizationProvider.Tests.ClassFieldsTests.LocalizedModelWithFields.AnotherField",
                         ExpressionHelper.GetFullMemberName(() => LocalizedModelWithFields.AnotherField));
        }

        [Fact]
        public void DiscoverNoClassField_OnlyIgnore()
        {
            var sut = new TypeDiscoveryHelper();

            var discoveredModels = sut.ScanResources(typeof(LocalizedModelWithOnlyIgnoredFields));

            // check return
            Assert.Empty(discoveredModels);
        }
    }
}
