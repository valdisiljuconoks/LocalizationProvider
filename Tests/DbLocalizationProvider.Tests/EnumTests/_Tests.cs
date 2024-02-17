using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.EnumTests;

public class LocalizedEnumTests
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly IEnumerable<DiscoveredResource> _properties;
    private readonly TypeDiscoveryHelper _sut;

    public LocalizedEnumTests()
    {
        var types = new[] { typeof(DocumentEntity) };
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

        Assert.NotEmpty(types);

        _properties = types.SelectMany(t => _sut.ScanResources(t));
    }

    [Fact]
    public void DiscoverEnumValue_NameAsTranslation()
    {
        var properties = _sut.ScanResources(typeof(SampleStatus));

        var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleStatus.Open");

        Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
    }

    [Fact]
    public void DiscoverEnumWithPrefixKey()
    {
        var properties = _sut.ScanResources(typeof(SampleStatusWithPrefix));

        var openStatus = properties.First(p => p.Key == "ThisIsPrefix.Open");

        Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
    }

    [Fact]
    public void EnumType_CheckDiscovered_Found()
    {
        var enumProperty =
            _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.DocumentEntity.Status");
        Assert.NotNull(enumProperty);
    }

    [Fact]
    public void EnumWithDisplayAttribute_TranslationEqualToSpecifiedInAttribute()
    {
        var properties = _sut.ScanResources(typeof(SampleEnumWithDisplayAttribute));

        var newStatus =
            properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleEnumWithDisplayAttribute.New");

        Assert.Equal("This is new", newStatus.Translations.DefaultTranslation());
    }

    [Fact]
    public void EnumWithAdditionalTranslation_DiscoversAllTranslations()
    {
        var properties = _sut.ScanResources(typeof(SampleEnumWithAdditionalTranslations));

        var openStatus =
            properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleEnumWithAdditionalTranslations.Open");

        Assert.Equal(3, openStatus.Translations.Count);
        Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
        Assert.Equal("Åpen", openStatus.Translations.First(t => t.Culture == "no").Translation);
    }

    [Fact]
    public void EnumWithResourceKeys_GeneratesKeysWithSpecifiedNames()
    {
        var discoveredResources = _sut.ScanResources(typeof(SampleEnumWithKeys));

        Assert.NotEmpty(discoveredResources);

        Assert.Contains(discoveredResources, r => r.Key == "/this/is/key");
    }

    [Fact]
    public void EnumWithResourceKeys_GeneratesKeysWithSpecifiedNames_WithClassPrefix()
    {
        var discoveredResources = _sut.ScanResources(typeof(SampleEnumWithKeysWithClassPrefix));

        Assert.NotEmpty(discoveredResources);

        Assert.Contains(discoveredResources, r => r.Key == "/this/is/prefix/and/this/is/key");
    }

    [Fact]
    public void Test_EnumExpression_AsProperty()
    {
        var doc = new DocumentEntity { Status = SampleStatus.New };

        Assert.Equal("DbLocalizationProvider.Tests.EnumTests.DocumentEntity.Status",
                     _expressionHelper.GetFullMemberName(() => doc.Status));
    }

    [Fact]
    public void Test_EnumExpression_Directly()
    {
        Assert.Equal("DbLocalizationProvider.Tests.EnumTests.SampleStatus.Open",
                     _expressionHelper.GetFullMemberName(() => SampleStatus.Open));
    }

    [Fact]
    public void Test_MemberAccessExpression()
    {
        var doc = new DocumentEntity();
        Assert.Equal("DbLocalizationProvider.Tests.EnumTests.DocumentEntity", _expressionHelper.GetFullMemberName(() => doc));
    }
}
