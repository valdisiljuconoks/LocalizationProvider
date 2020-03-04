using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ModelMetadataLocalizationHelperTests
    {
        public ModelMetadataLocalizationHelperTests()
        {
            ConfigurationContext.Current.EnableLegacyMode = () => true;
        }

        [Fact]
        public void UseLegacyMode_DisplayNameIsNull_ReturnsFalse()
        {
            var result = ConfigurationContext.Current.ResourceLookupFilter(null);

            Assert.False(result);
        }

        [Fact]
        public void DontUseLegacyMode_DisplayNameHasNotLegacyFormat_ReturnsTrue()
        {
            var displayName = "propertyName";

            ConfigurationContext.Current.EnableLegacyMode = () => false;

            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.True(result);
        }

        [Fact]
        public void UseLegacyMode_EnableLegacyModeIsFalse_ReturnsTrue()
        {
            var displayName = "propertyName";

            ConfigurationContext.Current.EnableLegacyMode = () => true;
            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.True(result);
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

        [Fact]
        public void DontUseLegacyMode_DisplayNameIsLegacyModeWithLegacyModeEnabled_ReturnsFalse()
        {
            var displayName = "/legacy/path";

            ConfigurationContext.Current.EnableLegacyMode = () => false;

            var result = ConfigurationContext.Current.ResourceLookupFilter(displayName);

            Assert.False(result);
        }
    }
}
