using Xunit;

namespace DbLocalizationProvider.EPiServer.JsResourceHandler.Tests
{
    public class CacheKeyTests
    {
        [Fact]
        public void TestKeyExtract_ExistingKey()
        {
            var key = CacheKeyHelper.GenerateKey("MyProject.MyResources", "en", true);
            var name = CacheKeyHelper.GetContainerName(key);

            Assert.Equal("MyProject.MyResources", name);
        }

        [Fact]
        public void TestKeyExtract_NonExisting()
        {
            var name = CacheKeyHelper.GetContainerName("asdf");

            Assert.Null(name);
        }
    }
}
