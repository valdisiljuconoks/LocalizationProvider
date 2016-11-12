using System;
using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedEnumsDiscoveryTests
    {
        private readonly List<Type> _types;

        public LocalizedEnumsDiscoveryTests()
        {
            _types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedResourceAttribute>().ToList();
            Assert.NotEmpty(_types);
        }

        [Fact]
        public void DiscoverEnumValue_NameAsTranslation()
        {
            var sut = new TypeDiscoveryHelper();
            var type = _types.First(t => t.FullName == "DbLocalizationProvider.Tests.SampleStatus");
            var properties = sut.ScanResources(type);

            var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.SampleStatus.Open");

            Assert.Equal("Open", openStatus.Translation);
        }
    }
}
