using DbLocalizationProvider.Import;
using Xunit;

namespace DbLocalizationProvider.Tests.ImporterTests
{
    public class ProviderDetectionTests
    {
        [Fact]
        public void DetectKnownProvider_ByFileExtension()
        {
            var sut = new ImportSettings();

            var foundProvider = sut.Providers.FindByExtension(".json");

            Assert.NotNull(foundProvider);
            Assert.Equal("json", foundProvider.ProviderId);
        }

        [Fact]
        public void DetectKnownProvider_ByFileExtension_UPPERCASE()
        {
            var sut = new ImportSettings();

            var foundProvider = sut.Providers.FindByExtension(".JSON");

            Assert.NotNull(foundProvider);
            Assert.Equal("json", foundProvider.ProviderId);
        }

        [Fact]
        public void DetectKnownProvider_ByOneOfFileExtensions()
        {
            var sut = new ImportSettings();
            sut.Providers.Add(new FormatParserMock());

            var foundProvider = sut.Providers.FindByExtension(".ext2");

            Assert.NotNull(foundProvider);
            Assert.Equal("ext", foundProvider.ProviderId);
        }

        [Fact]
        public void UnknownExtension_NoProviderFound()
        {
            var sut = new ImportSettings();

            var foundProvider = sut.Providers.FindByExtension(".asdf");

            Assert.Null(foundProvider);
        }
    }
}
