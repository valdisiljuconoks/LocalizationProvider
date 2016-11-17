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
            var sut = new TypeDiscoveryHelper();

            Assert.NotEmpty(types);

            _properties = types.SelectMany(t => sut.ScanResources(t));
        }

        private readonly IEnumerable<DiscoveredResource> _properties;

        [Fact]
        public void EnumType_CheckDiscovered_Found()
        {
            var enumProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.DocumentEntity.Status");
            Assert.NotNull(enumProperty);
        }

        [Fact]
        public void DiscoverEnumValue_NameAsTranslation()
        {
            var sut = new TypeDiscoveryHelper();
            var properties = sut.ScanResources(typeof(SampleStatus));

            var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.SampleStatus.Open");

            Assert.Equal("Open", openStatus.Translation);
        }
    }
}
