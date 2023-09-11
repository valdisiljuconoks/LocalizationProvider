using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations;

public class ModelMetadataLocalizationHelperTests
{
    [Fact]
    public void UseLegacyMode_DisplayNameIsNull_ReturnsFalse()
    {
        var ctx = new ConfigurationContext { EnableLegacyMode = () => true };

        var result = ctx.ShouldLookupResource(null);

        Assert.False(result);
    }

    [Fact]
    public void DontUseLegacyMode_DisplayNameHasNotLegacyFormat_ReturnsTrue()
    {
        var displayName = "propertyName";

        var ctx = new ConfigurationContext { EnableLegacyMode = () => false };

        var result = ctx.ShouldLookupResource(displayName);

        Assert.True(result);
    }

    [Fact]
    public void UseLegacyMode_EnableLegacyModeIsFalse_ReturnsTrue()
    {
        var displayName = "propertyName";

        var ctx = new ConfigurationContext { EnableLegacyMode = () => true };

        var result = ctx.ShouldLookupResource(displayName);

        Assert.True(result);
    }

    [Fact]
    public void UseLegacyMode_DisplayNameIsNotLegacyModeWithLegacyModeEnabled_ReturnsFalse()
    {
        var displayName = "propertyName";

        var ctx = new ConfigurationContext { ResourceLookupFilter = key => true };

        var result = ctx.ShouldLookupResource(displayName);

        Assert.True(result);
    }

    [Fact]
    public void UseLegacyMode_DisplayNameIsLegacyModeWithLegacyModeEnabled_ReturnsTrue()
    {
        var displayName = "/legacy/path";

        var ctx = new ConfigurationContext { EnableLegacyMode = () => true, ResourceLookupFilter = key => true };

        var result = ctx.ShouldLookupResource(displayName);

        Assert.True(result);
    }

    [Fact]
    public void DontUseLegacyMode_DisplayNameIsLegacyModeWithLegacyModeEnabled_ReturnsFalse()
    {
        var displayName = "/legacy/path";

        var ctx = new ConfigurationContext { EnableLegacyMode = () => false };

        var result = ctx.ShouldLookupResource(displayName);

        Assert.False(result);
    }
}
