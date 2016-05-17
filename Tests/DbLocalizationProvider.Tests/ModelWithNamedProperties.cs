using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class NamedResourceKeysTests
    {
        private readonly IEnumerable<DiscoveredResource> _properties;

        public NamedResourceKeysTests()
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>().ToList();

            Assert.NotEmpty(types);

            _properties = types.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false));
        }

        [Fact]
        public void TestSimple()
        {
            var nonexistingProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.ModelWithNamedProperties.PageHeader");
            var namedProperty = _properties.FirstOrDefault(p => p.Key == "/this/is/xpath/key");

            Assert.Null(nonexistingProperty);
            Assert.NotNull(namedProperty);

            Assert.Equal("This is page header", namedProperty.Translation);
        }
    }

    [LocalizedModel]
    public class ModelWithNamedProperties
    {
        [ResourceKey("/this/is/xpath/key", Value = "This is page header")]
        public virtual string PageHeader { get; set; }
    }

    //[LocalizedModel]
    //public class ModelWithNestedNamedProperties
    //{
    //    [ResourceKey("/this/is/xpath/key")]
    //    public virtual string PageHeader { get; set; }
    //}
}
