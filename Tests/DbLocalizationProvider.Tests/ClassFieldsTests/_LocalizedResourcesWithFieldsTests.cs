using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.ClassFieldsTests;

public class LocalizedResourcesWithFieldsTests
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly TypeDiscoveryHelper _sut;

    public LocalizedResourcesWithFieldsTests()
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

        _expressionHelper = new ExpressionHelper(keyBuilder);
    }

    [Fact]
    public void DiscoverClassField_WithDefaultValue()
    {
        var discoveredResources = _sut.ScanResources(typeof(LocalizedResourceWithFields));

        // check return
        Assert.NotEmpty(discoveredResources);

        // check discovered translation
        Assert.Equal("sample value", discoveredResources.First().Translations.DefaultTranslation());

        // check generated key from expression
        Assert.Equal("DbLocalizationProvider.Tests.ClassFieldsTests.LocalizedResourceWithFields.ThisisField",
                     _expressionHelper.GetFullMemberName(() => LocalizedResourceWithFields.ThisisField));
    }

    [Fact]
    public void DiscoverNoClassField_OnlyWithIgnore()
    {
        var discoveredResources = _sut.ScanResources(typeof(LocalizedResourceWithIgnoredFields));

        // check return
        Assert.Empty(discoveredResources);
    }
}
