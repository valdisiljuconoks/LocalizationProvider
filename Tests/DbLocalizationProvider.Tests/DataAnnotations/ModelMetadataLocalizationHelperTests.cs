using DbLocalizationProvider.DataAnnotations;
using System;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ModelMetadataLocalizationHelperTests
    {
        [Fact]
        public void UseLegacyMode_LocalizedDisplayNameIsNull_ReturnsFalse()
        {
            String localizedDisplayName = null;

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(localizedDisplayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_LocalizedDisplayNameDoesNotStartWithSlash_ReturnsFalse()
        {
            String localizedDisplayName = "displayName";

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(localizedDisplayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_EnableLegacyModeIsFalse_ReturnsFalse()
        {
            String localizedDisplayName = "/legacy/path";
            ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode = () => false;

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(localizedDisplayName);

            Assert.False(result);
        }

        [Fact]
        public void UseLegacyMode_LegazyKeyWithEnabledLegacyMode_ReturnsTrue()
        {
            String localizedDisplayName = "/legacy/path";
            ConfigurationContext.Current.ModelMetadataProviders.EnableLegacyMode = () => true;

            var result = ModelMetadataLocalizationHelper.UseLegacyMode(localizedDisplayName);

            Assert.True(result);
        }
    }
}
