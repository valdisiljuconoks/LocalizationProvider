using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using DbLocalizationProvider.Tests.DataAnnotations;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using Xunit;

namespace DbLocalizationProvider.Tests.DictionaryConvertTests;

public class _Tests
{
    private readonly LocalizationProvider _provider;

    public _Tests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(state, wrapper);

        ctx.TypeFactory
            .ForQuery<DetermineDefaultCulture.Query>()
            .SetHandler<DetermineDefaultCulture.Handler>()
            .ForQuery<GetTranslation.Query>()
            .SetHandler<GetTranslationReturnResourceKeyHandler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);

        var expressHelper = new ExpressionHelper(keyBuilder);
        _provider = new LocalizationProvider(keyBuilder,
                                             expressHelper,
                                             wrapper,
                                             queryExecutor,
                                             new ScanState());
    }

    [Fact]
    public void ConvertToDictionary()
    {
        var result = _provider.ToDictionary(typeof(ResourceToDictionaryModel));

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.True(result.ContainsKey("Property1"));
    }


    [Fact]
    public void ConvertToDictionary_HiddenResource_ShouldNotInclude()
    {
        var result = _provider.ToDictionary<ResourceToDictionaryModelWithHiddenProperty>();

        Assert.Single(result);
        Assert.True(result.ContainsKey("Property1"));
    }
}

[LocalizedResource]
public class ResourceToDictionaryModel
{
    public string Property1 { get; set; }
}

[LocalizedResource]
public class ResourceToDictionaryModelWithHiddenProperty
{
    public string Property1 { get; set; }

    [Hidden]
    public string Property2 { get; set; }
}
