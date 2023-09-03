using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources
{
    public class NamedResourcesTests
    {
        public NamedResourcesTests()
        {
            var state = new ScanState();
            var ctx = new ConfigurationContext
            {
                EnableLegacyMode = () => true
            };

            _keyBuilder = new ResourceKeyBuilder(state, ctx);
            var oldKeyBuilder = new OldResourceKeyBuilder(_keyBuilder);
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(_keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(_keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(_keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(_keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);

            _expressionHelper = new ExpressionHelper(_keyBuilder);
        }

        private readonly TypeDiscoveryHelper _sut;
        private readonly ExpressionHelper _expressionHelper;
        private readonly ResourceKeyBuilder _keyBuilder;

        [Fact]
        public async Task DuplicateAttributes_DiffProperties_SameKey_ThrowsException()
        {
            var models = new[] { typeof(BadResourceWithDuplicateKeysWithinClass) };
            var result = new List<DiscoveredResource>();

            await Assert.ThrowsAsync<DuplicateResourceKeyException>(async () =>
            {
                foreach (var model in models)
                {
                    result.AddRange(await _sut.ScanResources(model));
                }
            });
        }

        [Fact]
        public async Task ClassLevelResourceKeys_Discovers()
        {
            var model = typeof(ResourceWithClassLevelAttribute);
            var result = await _sut.ScanResources(model);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task DuplicateAttributes_SingleProperty_SameKey_ThrowsException()
        {
            var models = new[] { typeof(BadResourceWithDuplicateKeys) };
            var result = new List<DiscoveredResource>();

            await Assert.ThrowsAsync<DuplicateResourceKeyException>(async () =>
            {
                foreach (var model in models)
                {
                    result.AddRange(await _sut.ScanResources(model));
                }
            });
        }

        [Fact]
        public void ExpressionTest_WithNamedResources_NoPrefix_ReturnsResourceKey()
        {
            var result = _expressionHelper.GetFullMemberName(() => ResourcesWithNamedKeys.PageHeader);

            Assert.Equal("/this/is/xpath/to/resource", result);
        }

        [Fact]
        public void ExpressionTest_WithNamedResources_WithPrefix_ReturnsResourceKey()
        {
            var result = _expressionHelper.GetFullMemberName(() => ResourcesWithNamedKeysWithPrefix.PageHeader);

            Assert.Equal("/this/is/root/resource/and/this/is/header", result);
        }

        [Fact]
        public async Task MultipleAttributesForSingleProperty_NoPrefix()
        {
            var models = _sut.GetTypesWithAttribute<LocalizedResourceAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeys)}");

            var properties = new List<DiscoveredResource>();
            foreach (var model in models)
            {
                properties.AddRange(await _sut.ScanResources(model));
            }

            var namedResource = properties.Find(p => p.Key == "/this/is/xpath/to/resource");

            Assert.NotNull(namedResource);
            Assert.Equal("This is header", namedResource.Translations.DefaultTranslation());
        }

        [Fact]
        public async Task MultipleAttributesForSingleProperty_WithPrefix()
        {
            var models = _sut.GetTypesWithAttribute<LocalizedResourceAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ResourcesWithNamedKeysWithPrefix)}");

            var properties = new List<DiscoveredResource>();
            foreach (var model in models)
            {
                properties.AddRange(await _sut.ScanResources(model));
            }

            var namedResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/this/is/header");

            Assert.NotNull(namedResource);
            Assert.Equal("This is header", namedResource.Translations.DefaultTranslation());

            var firstResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/1stresource");

            Assert.NotNull(firstResource);
            Assert.Equal("Value in attribute", firstResource.Translations.DefaultTranslation());

            var secondResource = properties.FirstOrDefault(p => p.Key == "/this/is/root/resource/and/2ndresource");

            Assert.NotNull(secondResource);
            Assert.Equal("This is property value", secondResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void MultipleAttributesForSingleProperty_WithPrefix_KeyBuilderTest()
        {
            var key = _keyBuilder.BuildResourceKey(typeof(ResourcesWithNamedKeysWithPrefix),
                                                   nameof(ResourcesWithNamedKeysWithPrefix.SomeResource));

            Assert.Equal("/this/is/root/resource/SomeResource", key);
        }
    }
}
