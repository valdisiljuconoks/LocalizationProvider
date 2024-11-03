using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class ResourceKeyBuilderTests
{
    private readonly ResourceKeyBuilder _keyBuilder;

    public ResourceKeyBuilderTests()
    {
        var wrapper = new OptionsWrapper<ConfigurationContext>(new ConfigurationContext());
        _keyBuilder = new ResourceKeyBuilder(new ScanState(), wrapper);
    }

    [Fact]
    public void GetModelKey_OnlyByClass()
    {
        Assert.Equal("DbLocalizationProvider.Tests.SampleViewModel", _keyBuilder.BuildResourceKey(typeof(SampleViewModel)));
    }

    [Fact]
    public void GetResourceKey_OnlyByClass()
    {
        Assert.Equal("DbLocalizationProvider.Tests.ResourceKeys", _keyBuilder.BuildResourceKey(typeof(ResourceKeys)));
    }
}
