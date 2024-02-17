using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations;

public class ViewModelWithInheritanceTests
{
    [Fact]
    public void NotInheritedModel_ContainsOnlyDeclaredProperties()
    {
        var state = new ScanState();
        var ctx = new ConfigurationContext();
        var wrapper = new OptionsWrapper<ConfigurationContext>(ctx);
        var keyBuilder = new ResourceKeyBuilder(state, wrapper);
        var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
        ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

        var queryExecutor = new QueryExecutor(ctx.TypeFactory);
        var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

        var sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
                                          {
                                              new LocalizedModelTypeScanner(
                                                  keyBuilder,
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

        var properties = sut.ScanResources(typeof(SampleViewModelWithBase)).ToList();
        var keys = properties.Select(p => p.Key).ToList();
        var stringLengthResource =
            properties.FirstOrDefault(r => r.Key
                                           == "DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-StringLength");

        Assert.Contains("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description", keys);
        Assert.NotNull(stringLengthResource);
        Assert.Contains("StringLength", stringLengthResource.Translations.DefaultTranslation());
        Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty", keys);
        Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty-Required", keys);
        Assert.DoesNotContain(
            "DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description-Required",
            keys);
    }
}
