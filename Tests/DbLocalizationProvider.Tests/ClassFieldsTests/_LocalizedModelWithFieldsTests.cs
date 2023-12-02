using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.ClassFieldsTests;

public class LocalizedModelWithFieldsTests
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly TypeDiscoveryHelper _sut;

    public LocalizedModelWithFieldsTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        _keyBuilder = new ResourceKeyBuilder(state, wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(_keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(_keyBuilder,
                                                                         oldKeyBuilder,
                                                                         state,
                                                                         wrapper,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               wrapper,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(_keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               wrapper,
                                               translationBuilder)
                                       },
                                       wrapper);

        _expressionHelper = new ExpressionHelper(_keyBuilder);
    }

    [Fact]
    public void DiscoverClassField_ChildClassWithNoInherit_FieldIsNotInChildClassNamespace()
    {
        var discoveredModels = new[] { typeof(LocalizedChildModelWithFields), typeof(LocalizedBaseModelWithFields) }
            .SelectMany(t => _sut.ScanResources(t))
            .ToList();

        // check return
        Assert.NotEmpty(discoveredModels);
    }

    [Fact]
    public void DiscoverClassField_OnlyIncluded()
    {
        var discoveredModels = _sut.ScanResources(typeof(LocalizedModelWithOnlyIncludedFields));

        // check return
        Assert.NotEmpty(discoveredModels);

        // check translation
        Assert.Equal("yet other value", discoveredModels.First().Translations.DefaultTranslation());
    }

    [Fact]
    public void DiscoverClassField_WithDefaultValue()
    {
        var discoveredModels = _sut.ScanResources(typeof(LocalizedModelWithFields));

        // check return
        Assert.NotEmpty(discoveredModels);

        // check discovered translation
        Assert.Equal("other value", discoveredModels.First().Translations.DefaultTranslation());

        //// check generated key from expression
        Assert.Equal("DbLocalizationProvider.Tests.ClassFieldsTests.LocalizedModelWithFields.AnotherField",
                     _expressionHelper.GetFullMemberName(() => LocalizedModelWithFields.AnotherField));
    }

    [Fact]
    public void DiscoverClassInstanceField()
    {
        var t = new LocalizedModelWithInstanceField();

        var discoveredModels = _sut.ScanResources(t.GetType());

        // check return
        Assert.NotEmpty(discoveredModels);

        // check discovered translation
        Assert.Equal("instance field value", discoveredModels.First().Translations.DefaultTranslation());

        Assert.Equal("DbLocalizationProvider.Tests.ClassFieldsTests.LocalizedModelWithInstanceField.ThisIsInstanceField",
                     _expressionHelper.GetFullMemberName(() => t.ThisIsInstanceField));
    }

    [Fact]
    public void DiscoverClassField_RespectsResourceKeyAttribute()
    {
        var discoveredModels = _sut.ScanResources(typeof(LocalizedModelWithFieldResourceKeys));

        // check return
        Assert.NotEmpty(discoveredModels);

        // check discovered translation
        Assert.Equal("/this/is/key", discoveredModels.First().Key);

        Assert.Equal("/this/is/key",
                     _keyBuilder.BuildResourceKey(typeof(LocalizedModelWithFieldResourceKeys),
                                                  nameof(LocalizedModelWithFieldResourceKeys.AnotherField)));
    }

    [Fact]
    public void DiscoverNoClassField_OnlyIgnore()
    {
        var discoveredModels = _sut.ScanResources(typeof(LocalizedModelWithOnlyIgnoredFields));

        // check return
        Assert.Empty(discoveredModels);
    }
}
