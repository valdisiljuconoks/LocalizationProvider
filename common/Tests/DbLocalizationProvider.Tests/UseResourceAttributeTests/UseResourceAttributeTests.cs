using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using DbLocalizationProvider.Tests.JsonConverterTests;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.UseResourceAttributeTests;

public class UseResourceAttributeTests
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly TypeDiscoveryHelper _sut;
    private readonly LocalizationProvider _providerUnderTests;
    private readonly ConfigurationContext _ctx;

    public UseResourceAttributeTests()
    {
        var state = new ScanState();
        _ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(_ctx);
        var keyBuilder = new ResourceKeyBuilder(state, wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        _ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(_ctx.TypeFactory);
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

        var ctx = new ConfigurationContext();
        ctx.FallbackLanguages.Try(CultureInfo.GetCultureInfo("en"));

        _providerUnderTests = new LocalizationProvider(keyBuilder,
                                     _expressionHelper,
                                     new OptionsWrapper<ConfigurationContext>(ctx),
                                     queryExecutor, 
                                     state);
    }

    [Fact]
    public void UseResourceAttribute_NoResourceRegistered_ResolvedTargetResourceKey()
    {
        var m = new ModelWithOtherResourceUsage();

        _sut.ScanResources(typeof(ModelWithOtherResourceUsage));

        var resultKey = _expressionHelper.GetFullMemberName(() => m.SomeProperty);

        Assert.Equal("DbLocalizationProvider.Tests.UseResourceAttributeTests.CommonResources.CommonProp", resultKey);
    }
    
    [Fact]
    public void UseResourceAttribute_TranslateTargetType_ShouldUseCorrectResource()
    {
        var results = new[] { typeof(CommonResources), typeof(ModelWithOtherResourceUsage) }
            .SelectMany(t => _sut.ScanResources(t))
            .ToList();

        _ctx.TypeFactory.ForQuery<GetAllResources.Query>().SetHandler(() => new GetAllResourcesUnitTestHandler(results));

        var resultTranslatedObject = _providerUnderTests.Translate<ModelWithOtherResourceUsage>();

        Assert.Equal("Common property translation", resultTranslatedObject.SomeProperty);
    }
}
