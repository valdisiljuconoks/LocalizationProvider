using System.Collections.Generic;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.RecursiveModelsTests
{
    public class _Tests
    {
        private readonly TypeDiscoveryHelper _sut;

        public _Tests()
        {
            var state = new ScanState();
            var keyBuilder = new ResourceKeyBuilder(state);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            var ctx = new ConfigurationContext();
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);
        }

        [Fact]
        public void Model_WithTheSameModelAsProperty_ShouldThrow()
        {
            Assert.Throws<RecursiveResourceReferenceException>(() =>
            {
                var resources = _sut.ScanResources(typeof(Person));
            });
        }

        [Fact]
        public void Model_WithObjectProperty_ShouldNotThrow()
        {
            var resources = _sut.ScanResources(typeof(ResourceClassWithObjectTypeProperty));
        }
    }

    [LocalizedModel]
    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Person Mother { get; set; }
        public Person Father { get; set; }
    }

    [LocalizedModel]
    public class ResourceClassWithObjectTypeProperty
    {
        public object SomeObject { get; set; } = "test";
    }
}
