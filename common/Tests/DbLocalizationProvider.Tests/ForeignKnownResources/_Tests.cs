using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.ForeignKnownResources;

public class ForeignResourceScannerTests
{
    private readonly TypeDiscoveryHelper _sut;

    public ForeignResourceScannerTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(state, wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        ctx.ForeignResources
            .Add<ResourceWithNoAttribute>()
            .Add<BadRecursiveForeignResource>(true);

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(keyBuilder,
                                                                         oldKeyBuilder,
                                                                         state,
                                                                         wrapper,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               wrapper,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               wrapper,
                                               translationBuilder)
                                       },
                                       wrapper);
    }

    [Fact]
    public void DiscoverForeignResourceClass_SingleProperty()
    {
        var resources = _sut.ScanResources(typeof(ResourceWithNoAttribute));

        Assert.True(resources.Any());

        var resource = resources.First();

        Assert.Equal("Default resource value", resource.Translations.DefaultTranslation());
        Assert.Equal("DbLocalizationProvider.Tests.ForeignKnownResources.ResourceWithNoAttribute.SampleProperty", resource.Key);
    }

    [Fact]
    public void DiscoverForeignResourceNestedClass()
    {
        var resources = _sut.ScanResources(typeof(ResourceWithNoAttribute.NestedResource));

        Assert.True(resources.Any());

        var resource = resources.First();

        Assert.Equal("NestedProperty", resource.Translations.DefaultTranslation());
        Assert.Equal("DbLocalizationProvider.Tests.ForeignKnownResources.ResourceWithNoAttribute+NestedResource.NestedProperty",
                     resource.Key);
    }

    [Fact]
    public void DiscoverForeignResource_Enum()
    {
        var resources = _sut.ScanResources(typeof(SomeEnum));

        Assert.True(resources.Any());
        Assert.Equal(3, resources.Count());

        var resource = resources.First();

        Assert.Equal("None", resource.Translations.DefaultTranslation());
        Assert.Equal("DbLocalizationProvider.Tests.ForeignKnownResources.SomeEnum.None", resource.Key);
    }

    [Fact]
    public void ScanStackOverflowResource_WithPropertyReturningSameDeclaringType_ViaForeignResources()
    {
        var results = _sut.ScanResources(typeof(BadRecursiveForeignResource));

        Assert.NotNull(results);
        Assert.Single(results);
    }
}
