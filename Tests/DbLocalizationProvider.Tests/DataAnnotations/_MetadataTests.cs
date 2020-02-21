using DbLocalizationProvider.DataAnnotations;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ModelMetadataLocalizationHelperTests
    {
        [Fact]
        public void UseLegacyMode_DisplayNameIsNull_ReturnsFalse()
        {
            var result = ConfigurationContext.Current.ResourceLookupFilter(null);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_DisplayNameHasNotLegacyFormat_ReturnsFalse()
        {
            var displayName = "propertyName";

            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_EnableLegacyModeIsFalse_ReturnsFalse()
        {
            var displayName = "propertyName";

            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_DisplayNameIsNotLegacyModeWithLegacyModeEnabled_ReturnsFalse()
        {
            var displayName = "propertyName";

            ConfigurationContext.Current.ResourceLookupFilter = key => true;

            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.True(result);
        }

        [Fact]
        public void UseLegacyMode_DisplayNameIsLegacyModeWithLegacyModeEnabled_ReturnsTrue()
        {
            var displayName = "/legacy/path";

            ConfigurationContext.Current.ResourceLookupFilter = key => true;

            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.True(result);
        }
    }
}
