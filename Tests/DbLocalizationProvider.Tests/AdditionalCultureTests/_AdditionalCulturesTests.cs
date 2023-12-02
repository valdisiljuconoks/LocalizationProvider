using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.AdditionalCultureTests;

public class AdditionalCulturesTests
{
    private readonly TypeDiscoveryHelper _sut;

    public AdditionalCulturesTests()
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
    public void DiscoverAdditionalTranslations()
    {
        var results = _sut.ScanResources(typeof(SomeResources));

        Assert.NotEmpty(results);
        Assert.Equal("Name", results.First().Translations.DefaultTranslation());
    }

    [Fact]
    public void DiscoverAdditionalTranslations_ForResourceWithKeys()
    {
        var results = _sut.ScanResources(typeof(SomeResourcesWithKeys));

        Assert.NotEmpty(results);
        Assert.Equal("Noen i norsk", results.First().Translations.First(t => t.Culture == "no").Translation);
    }

    [Fact]
    public void DiscoverAdditionalTranslations_FromEmum()
    {
        var results = _sut.ScanResources(typeof(SomeEnumResource));

        Assert.NotEmpty(results);
        Assert.Equal("Name", results.First().Translations.DefaultTranslation());
    }

    [Fact]
    public void ThrowOnDuplicateCultures_FromEnum()
    {
        Assert.Throws<DuplicateResourceTranslationsException>(
            () => _sut.ScanResources(typeof(SomeEnumResourceWithDuplicateCultures)));
    }

    [Fact]
    public void ThrowOnDuplicateCultures_FromOrdinaryResource()
    {
        Assert.Throws<DuplicateResourceTranslationsException>(
            () => _sut.ScanResources(typeof(SomeResourcesWithDuplicateCultures)));
    }

    [Fact]
    public void ScanResource_BadTranslationLanguage()
    {
        Assert.Throws<CultureNotFoundException>(() => _sut.ScanResources(typeof(BadResourceWithNoExistingLanguageCode)));
    }
}
