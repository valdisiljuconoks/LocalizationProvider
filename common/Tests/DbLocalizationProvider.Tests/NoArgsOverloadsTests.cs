using System.Collections.Generic;
using System.Globalization;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests;

public class NoArgsOverloadsTests
{
    private const string PlainKey = "DbLocalizationProvider.Tests.NoArgs.Plain";
    private const string CurlyKey = "DbLocalizationProvider.Tests.NoArgs.Curly";

    private readonly LocalizationProvider _sut;

    public NoArgsOverloadsTests()
    {
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(new ScanState(), wrapper);

        ctx.FallbackLanguages.Try(CultureInfo.GetCultureInfo("en"));

        ctx.TypeFactory.ForQuery<GetTranslation.Query>()
            .SetHandler(() => new StaticTranslationHandler(new Dictionary<string, string>
            {
                [PlainKey] = "Hello world",
                [CurlyKey] = "value with {literal} braces"
            }));

        IQueryExecutor queryExecutor = new QueryExecutor(ctx.TypeFactory);

        _sut = new LocalizationProvider(keyBuilder,
                                        new ExpressionHelper(keyBuilder),
                                        wrapper,
                                        queryExecutor,
                                        new ScanState());
    }

    [Fact]
    public void GetStringByCulture_NoArgs_MatchesParamsOverload_WhenNoFormatArgs()
    {
        var en = CultureInfo.GetCultureInfo("en");
        Assert.Equal(_sut.GetStringByCulture(PlainKey, en, []),
                     _sut.GetStringByCulture(PlainKey, en));
    }

    [Fact]
    public void GetStringByCulture_NoArgs_PreservesCurlyBraces()
    {
        // The no-args path skips string.Format and must return the raw value
        // even when it contains literal braces (which would be a placeholder pattern in the params path).
        Assert.Equal("value with {literal} braces",
                     _sut.GetStringByCulture(CurlyKey, CultureInfo.GetCultureInfo("en")));
    }

    [Fact]
    public void GetStringByCulture_NoArgs_NullCulture_Throws()
    {
        Assert.Throws<System.ArgumentNullException>(
            () => _sut.GetStringByCulture(PlainKey, null));
    }

    [Fact]
    public void GetStringByCulture_NoArgs_EmptyKey_Throws()
    {
        Assert.Throws<System.ArgumentNullException>(
            () => _sut.GetStringByCulture(string.Empty, CultureInfo.GetCultureInfo("en")));
    }

    private sealed class StaticTranslationHandler(Dictionary<string, string> store)
        : IQueryHandler<GetTranslation.Query, string>
    {
        public string Execute(GetTranslation.Query query)
        {
            return store.TryGetValue(query.Key, out var value) ? value : null;
        }
    }
}
