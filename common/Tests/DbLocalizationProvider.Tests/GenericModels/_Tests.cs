using System.Collections.Generic;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.GenericModels;

public class GenericModelTests
{
    private readonly TypeDiscoveryHelper _sut;
    private readonly ResourceKeyBuilder _keyBuilder;
    private readonly ScanState _state;
    private readonly OptionsWrapper<ConfigurationContext> _wrapper;

    public GenericModelTests()
    {
        _state = new ScanState();
        var ctx = new ConfigurationContext();
        _wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        _keyBuilder = new ResourceKeyBuilder(_state, _wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(_keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                       {
                                           new LocalizedModelTypeScanner(_keyBuilder,
                                                                         oldKeyBuilder,
                                                                         _state,
                                                                         _wrapper,
                                                                         translationBuilder),
                                           new LocalizedResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               _state,
                                               _wrapper,
                                               translationBuilder),
                                           new LocalizedEnumTypeScanner(_keyBuilder, translationBuilder),
                                           new LocalizedForeignResourceTypeScanner(
                                               _keyBuilder,
                                               oldKeyBuilder,
                                               _state,
                                               _wrapper,
                                               translationBuilder)
                                       },
                                       _wrapper);
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
        var key = new ExpressionHelper(_keyBuilder).GetFullMemberName(() => model.BaseProperty);

        Assert.Equal("DbLocalizationProvider.Tests.GenericModels.OpenGenericBase`1.BaseProperty", key);
    }
}
