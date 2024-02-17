using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.ResourcesAndInheritance;

public class InheritanceTests
{
    private readonly TypeDiscoveryHelper _sut;

    public InheritanceTests()
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
    public void ResourceWithBaseClass_NoInheritance_ScannedOnlyDirectProperties()
    {
        var result = _sut.ScanResources(typeof(ResourceWithBaseClassNoInheritance));

        Assert.Equal(1, result.Count());
    }

    [Fact]
    public void ResourceWithBaseClass_ScannedAll()
    {
        var result = _sut.ScanResources(typeof(ResourceWithBaseClass));

        Assert.Equal(2, result.Count());
    }
}
