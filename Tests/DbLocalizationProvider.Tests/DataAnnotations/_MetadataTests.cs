using DbLocalizationProvider.DataAnnotations;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ModelMetadataLocalizationHelperTests
    {
        [Fact]
        public void UseLegacyMode_DisplayNameIsNull_ReturnsFalse()
        {
            var result = ModelMetadataLocalizationHelper.UseLegacyMode(null);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_DisplayNameHasNotLegacyFormat_ReturnsFalse()
        {
            var displayName = "propertyName";

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(displayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_EnableLegacyModeIsFalse_ReturnsFalse()
        {
            var displayName = "propertyName";

            ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode = () => false;

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(displayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_DisplayNameIsNotLegacyModeWithLegacyModeEnabled_ReturnsFalse()
        {
            var displayName = "propertyName";

            ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode = () => true;

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(displayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_DisplayNameIsLegacyModeWithLegacyModeEnabled_ReturnsTrue()
        {
            var displayName = "/legacy/path";

            ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode = () => true;

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(displayName);

            Assert.True(result);
        }
    }
}
