using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class FormattedResourcesTests
    {
        [Fact]
        public void GetFormattedResourceValue()
        {
            var service = new UnitTestLocalizationService("Here should be `{0}` parameter");

            var result = service.GetString(() => ResourceKeys.ThisIsConstant, "param1");

            Assert.Equal("Here should be `param1` parameter", result);
        }
    }
}
