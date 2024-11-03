using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.DiscoveryTests;

public class ViewModelWithIncludedOnlyTests
{
    private readonly TypeDiscoveryHelper _sut;

    public ViewModelWithIncludedOnlyTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(state, wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

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
    public void ModelWithBase_IncludedPorperty_ShouldDiscoverOnlyExplicitProperties()
    {
        var properties = _sut.ScanResources(typeof(SampleViewModelWithIncludedOnlyWithBase))
            .Select(p => p.Key)
            .ToList();

        Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.IncludedProperty",
                        properties);
        Assert.DoesNotContain(
            "DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.ExcludedProperty",
            properties);

        Assert.Contains(
            "DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.IncludedBaseProperty",
            properties);
        Assert.DoesNotContain(
            "DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnlyWithBase.ExcludedBaseProperty",
            properties);
    }

    [Fact]
    public void ModelWithIncludedProperty_ShouldDiscoverOnlyExplicitProperties()
    {
        var properties = _sut.ScanResources(typeof(SampleViewModelWithIncludedOnly))
            .Select(p => p.Key)
            .ToList();

        Assert.Contains("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.IncludedProperty",
                        properties);
        Assert.DoesNotContain("DbLocalizationProvider.Tests.DiscoveryTests.SampleViewModelWithIncludedOnly.ExcludedProperty",
                              properties);
    }
}
