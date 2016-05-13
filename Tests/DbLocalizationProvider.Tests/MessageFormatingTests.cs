using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class MessageFormatingTests
    {
        [Fact]
        public void FormatMessageWithNamedPlaceholders()
        {
            var message = "Hello, {FirstName}";

            var result = LocalizationServiceExtensions.Format(message, new { FirstName = "John" });

            Assert.Equal("Hello, John", result);
        }

        [Fact]
        public void FormatMessageWithIndexPlaceholders()
        {
            var message = "Hello, {0}";

            var result = LocalizationServiceExtensions.Format(message, "John");

            Assert.Equal("Hello, John", result);
        }
    }
}
