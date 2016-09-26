using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedEnumTests
    {
        public LocalizedEnumTests()
        {
            var types = new[] { typeof(DocumentEntity) };

            Assert.NotEmpty(types);

            _properties = types.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false));
        }

        private readonly IEnumerable<DiscoveredResource> _properties;

        [Fact]
        public void EnumType_CheckDiscovered_Found()
        {
            var enumProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.DocumentEntity.Status");
            Assert.NotNull(enumProperty);
        }
    }
}
