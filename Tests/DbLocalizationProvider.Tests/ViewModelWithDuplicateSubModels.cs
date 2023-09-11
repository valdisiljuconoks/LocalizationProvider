using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    [LocalizedModel]
    public class ViewModelWithDuplicateSubModels
    {
        public SubModel SubModelPorperty1 { get; set; }
        public SubModel SubModelPorperty2 { get; set; }
    }

    [LocalizedModel]
    public class SubModel
    {
        [StringLength(5)]
        public string MyProperty { get; set; }
    }

    public class ReusedViewModelTests
    {
        [Fact]
        public void SameModel_MultipleDefinitions_DoesNotThrowException()
        {
            var state = new ScanState();
            var ctx = new ConfigurationContext();
            var keyBuilder = new ResourceKeyBuilder(state, ctx);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            var sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);

            var resources = sut.ScanResources(typeof(ViewModelWithDuplicateSubModels));

            Assert.NotNull(resources);

            var count = resources.Count(r => r.Key == "DbLocalizationProvider.Tests.SubModel.MyProperty-StringLength");

            Assert.Equal(1, count);
        }
    }
}
