using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources;

public class ComplexNestedResourceTests
{
    private readonly TypeDiscoveryHelper _sut;

    public ComplexNestedResourceTests()
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
    public void ComplexProperty_OnClassWithKey_PropertyGetsCorrectKey()
    {
        var result = _sut.ScanResources(typeof(ResourcesWithKeyAndComplexProperties));

        Assert.NotEmpty(result);
        Assert.Equal("Prefix.NestedProperty.SomeProperty", result.First().Key);
    }

    [Fact]
    public void ComplexProperty_OnClassWithKey_ExprEvaluatesCorrectKey()
    {
        var wrapper = new OptionsWrapper<ConfigurationContext>(new ConfigurationContext());
        var key = new ExpressionHelper(new ResourceKeyBuilder(new ScanState(), wrapper))
            .GetFullMemberName(() => ResourcesWithKeyAndComplexProperties.NestedProperty.SomeProperty);

        Assert.Equal("Prefix.NestedProperty.SomeProperty", key);
    }
}
