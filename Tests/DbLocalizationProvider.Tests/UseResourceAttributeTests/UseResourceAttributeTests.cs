using System.Collections.Generic;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.UseResourceAttributeTests;

public class UseResourceAttributeTests
{
    private readonly ExpressionHelper _expressionHelper;
    private readonly TypeDiscoveryHelper _sut;

    public UseResourceAttributeTests()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var keyBuilder = new ResourceKeyBuilder(state, ctx);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(keyBuilder,
                                                                         oldKeyBuilder,
                                                                         state,
                                                                         ctx,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               keyBuilder,
                                               oldKeyBuilder,
                                               state,
                                               ctx,
                                               translationBuilder)
                                       },
                                       ctx);

        _expressionHelper = new ExpressionHelper(keyBuilder);
    }

    [Fact]
    public void UseResourceAttribute_NoResourceRegistered()
    {
        var results = _sut.ScanResources(typeof(ModelWithOtherResourceUsage));

        Assert.Empty(results);
    }

    [Fact]
    public void UseResourceAttribute_NoResourceRegistered_ResolvedTargetResourceKey()
    {
        var m = new ModelWithOtherResourceUsage();

        _sut.ScanResources(typeof(ModelWithOtherResourceUsage));

        var resultKey = _expressionHelper.GetFullMemberName(() => m.SomeProperty);

        Assert.Equal("DbLocalizationProvider.Tests.UseResourceAttributeTests.CommonResources.CommonProp", resultKey);
    }
}
