using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ResourceKeyBuilderTests
    {
        private readonly ResourceKeyBuilder _keyBuilder;

        public ResourceKeyBuilderTests()
        {
            _keyBuilder = new ResourceKeyBuilder(new ScanState());
        }

        [Fact]
        public void GetModelKey_OnlyByClass()
        {
            Assert.Equal("DbLocalizationProvider.Tests.SampleViewModel", _keyBuilder.BuildResourceKey(typeof(SampleViewModel)));
        }

        [Fact]
        public void GetResourceKey_OnlyByClass()
        {
            Assert.Equal("DbLocalizationProvider.Tests.ResourceKeys", _keyBuilder.BuildResourceKey(typeof(ResourceKeys)));
        }
    }
}
