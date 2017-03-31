using DbLocalizationProvider.Internal;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ResourceKeyBuilderTests
    {
        [Fact]
        public void GetModelKey_OnlyByClass()
        {
            Assert.Equal("DbLocalizationProvider.Tests.SampleViewModel", ResourceKeyBuilder.BuildResourceKey(typeof(SampleViewModel)));
        }

        [Fact]
        public void GetResourceKey_OnlyByClass()
        {
            Assert.Equal("DbLocalizationProvider.Tests.ResourceKeys", ResourceKeyBuilder.BuildResourceKey(typeof(ResourceKeys)));
        }
    }
}
