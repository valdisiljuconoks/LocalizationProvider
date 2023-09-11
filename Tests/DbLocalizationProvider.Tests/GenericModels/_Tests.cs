using System.Collections.Generic;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.GenericModels;

public class GenericModelTests
{
    private readonly TypeDiscoveryHelper _sut;

    public GenericModelTests()
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
    }

    [Fact]
    public void TestGenericProperty()
    {
        var properties = _sut.ScanResources(typeof(OpenGenericModel<>));

        Assert.NotEmpty(properties);
    }

    [Fact]
    public void TestGenericProperty_FromChildClass()
    {
        var properties = _sut.ScanResources(typeof(ClosedGenericModel));

        Assert.NotEmpty(properties);
    }

    [Fact]
    public void TestGenericProperty_FromChildClass_WithNoInherit()
    {
        var properties1 = _sut.ScanResources(typeof(OpenGenericBase<>));
        var properties2 = _sut.ScanResources(typeof(CloseGenericNoInherit));

        Assert.NotEmpty(properties1);
        Assert.NotEmpty(properties2);

        var model = new CloseGenericNoInherit();
        var key =
            new ExpressionHelper(new ResourceKeyBuilder(new ScanState(), new ConfigurationContext()))
                .GetFullMemberName(() => model.BaseProperty);

        Assert.Equal("DbLocalizationProvider.Tests.GenericModels.OpenGenericBase`1.BaseProperty", key);
    }
}
