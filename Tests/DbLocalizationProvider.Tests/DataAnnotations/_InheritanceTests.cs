using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class ViewModelWithInheritanceTests
    {
        [Fact]
        public void NotInheritedModel_ContainsOnlyDeclaredProperties()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            var sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);

            var properties = sut.ScanResources(typeof(SampleViewModelWithBase)).ToList();
            var keys = properties.Select(p => p.Key).ToList();
            var stringLengthResource = properties.FirstOrDefault(r => r.Key == "DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-StringLength");

            Assert.Contains("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description", keys);
            Assert.NotNull(stringLengthResource);
            Assert.Contains("StringLength", stringLengthResource.Translations.DefaultTranslation());
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty", keys);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.BaseProperty-Required", keys);
            Assert.DoesNotContain("DbLocalizationProvider.Tests.DataAnnotations.SampleViewModelWithBase.ChildProperty-Description-Required", keys);
        }
    }
}
