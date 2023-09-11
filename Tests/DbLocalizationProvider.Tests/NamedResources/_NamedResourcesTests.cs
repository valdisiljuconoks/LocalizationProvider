using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources;

public class NamedResourcesTests
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly ResourceKeyBuilder _keyBuilder;

    private readonly TypeDiscoveryHelper _sut;

    public NamedResourcesTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext { EnableLegacyMode = () => true };

        _keyBuilder = new ResourceKeyBuilder(state, ctx);
        var oldKeyBuilder = new OldResourceKeyBuilder(_keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(_keyBuilder,
                                                                         oldKeyBuilder,
                                                                         state,
                                                                         ctx,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(_keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder)
                                       },
                                       ctx);

        _expressionHelper = new ExpressionHelper(_keyBuilder);
    }

    [Fact]
    public void DuplicateAttributes_DiffProperties_SameKey_ThrowsException()
    {
        var model = new[] { typeof(BadResourceWithDuplicateKeysWithinClass) };
        Assert.Throws<DuplicateResourceKeyException>(() => model.SelectMany(t => _sut.ScanResources(t)).ToList());
    }

    [Fact]
    public void ClassLevelResourceKeys_Discovers()
    {
        var model = typeof(ResourceWithClassLevelAttribute);
        var result = _sut.ScanResources(model);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void DuplicateAttributes_SingleProperty_SameKey_ThrowsException()
    {
        var model = new[] { typeof(BadResourceWithDuplicateKeys) };
        Assert.Throws<DuplicateResourceKeyException>(() => model.SelectMany(t => _sut.ScanResources(t)).ToList());
    }

    [Fact]
    public void ExpressionTest_WithNamedResources_NoPrefix_ReturnsResourceKey()
    {
        var result = _expressionHelper.GetFullMemberName(() => ResourcesWithNamedKeys.PageHeader);

        Assert.Equal("/this/is/xpath/to/resource", result);
    }

    [Fact]
    public void ExpressionTest_WithNamedResources_WithPrefix_ReturnsResourceKey()
    {
        var result = _expressionHelper.GetFullMemberName(() => ResourcesWithNamedKeysWithPrefix.PageHeader);

        Assert.Equal("/this/is/root/resource/and/this/is/header", result);
    }

    [Fact]
    public void MultipleAttributesForSingleProperty_NoPrefix()
    {
        var model = _sut.GetTypesWithAttribute<LocalizedResourceAttribute>()
            .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeys)}");

        var properties = model.SelectMany(t => _sut.ScanResources(t)).ToList();

        var namedResource = properties.FirstOrDefault(p => p.Key == "/this/is/xpath/to/resource");

        Assert.NotNull(namedResource);
        Assert.Equal("This is header", namedResource.Translations.DefaultTranslation());
    }

    [Fact]
    public void MultipleAttributesForSingleProperty_WithPrefix()
    {
        var model = _sut.GetTypesWithAttribute<LocalizedResourceAttribute>()
            .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeysWithPrefix)}");

        var properties = model.SelectMany(t => _sut.ScanResources(t)).ToList();

        var namedResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/this/is/header");

        Assert.NotNull(namedResource);
        Assert.Equal("This is header", namedResource.Translations.DefaultTranslation());

        var firstResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/1stresource");

        Assert.NotNull(firstResource);
        Assert.Equal("Value in attribute", firstResource.Translations.DefaultTranslation());

        var secondResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/2ndresource");

        Assert.NotNull(secondResource);
        Assert.Equal("This is property value", secondResource.Translations.DefaultTranslation());
    }

    [Fact]
    public void MultipleAttributesForSingleProperty_WithPrefix_KeyBuilderTest()
    {
        var key = _keyBuilder.BuildResourceKey(typeof(ResourcesWithNamedKeysWithPrefix),
                                               nameof(ResourcesWithNamedKeysWithPrefix.SomeResource));

        Assert.Equal("/this/is/root/resource/SomeResource", key);
    }
}
