using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.ScalarCollectionTests;

public class ScalarCollectionTests
{
    private readonly TypeDiscoveryHelper _sut;

    public ScalarCollectionTests()
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
    public void ScanResourceWillScalarEnumerables_ShouldDiscover()
    {
        var properties = _sut.ScanResources(typeof(ResourceClassWithScalarCollection));

        Assert.Equal(2, properties.Count());
    }

    [Fact]
    public void ScanModelWillScalarEnumerables_ShouldDiscover()
    {
        var properties = _sut.ScanResources(typeof(ModelClassWithScalarCollection));

        Assert.Equal(2, properties.Count());
    }
}

[LocalizedResource]
public class ResourceClassWithScalarCollection
{
    public int[] ArrayOfItns { get; set; }

    public List<string> CollectionOfStrings { get; set; }
}

[LocalizedModel]
public class ModelClassWithScalarCollection
{
    public int[] ArrayOfItns { get; set; }

    public List<string> CollectionOfStrings { get; set; }
}
