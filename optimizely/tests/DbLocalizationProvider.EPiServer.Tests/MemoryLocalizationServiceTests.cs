using System.Globalization;
using EPiServer.Framework.Localization;
using Xunit;

namespace DbLocalizationProvider.EPiServer.Tests
{
    public class MemoryLocalizationServiceTests
    {
        [Fact]
        public void TestWithEpiserverMemoryProvider()
        {
            var localizationService = new MemoryLocalizationService();
            localizationService.AddString(CultureInfo.CurrentUICulture, "DbLocalizationProvider.EPiServer.Tests.MyResource.SomeLabel", "Some label");
            localizationService.AddString(CultureInfo.CurrentUICulture, "DbLocalizationProvider.EPiServer.Tests.MyResource.SomeLabelWithPlaceholder", "Some label `{Name}`");

            var result = localizationService.GetString(() => MyResource.SomeLabel);
            Assert.Equal("Some label", result);

            var result2 = localizationService.GetString(() => MyResource.SomeLabelWithPlaceholder, new { Name = "John" });
            Assert.Equal("Some label `John`", result2);
        }
    }

    public class MyResource
    {
        public static string SomeLabel { get; set; } = "Some label (from class)";
        public static string SomeLabelWithPlaceholder { get; set; } = "Some label with placeholder (from class)";
    }
}
