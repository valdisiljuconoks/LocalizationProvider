using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.KnownAttributesTests
{
    public class CustomAttributeScannerTests
    {
        private readonly TypeDiscoveryHelper _sut;

        public CustomAttributeScannerTests()
        {
            var state = new ScanState();
            var ctx = new ConfigurationContext();
            var keyBuilder = new ResourceKeyBuilder(state, ctx);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            ctx.TypeFactory.ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>();
            ctx.CustomAttributes
                .Add<HelpTextAttribute>()
                .Add<FancyHelpTextAttribute>()
                .Add<AttributeWithDefaultTranslationAttribute>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
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
        public async Task ModelWith2ChildModelAsProperties_ReturnsDuplicates()
        {
            var types = new[]
            {
                typeof(ModelWithTwoChildModelPropertiesCustomAttributes),
                typeof(AnotherModelWithTwoChildModelPropertiesCustomAttributes)
            };

            var result = new List<DiscoveredResource>();
            foreach (var type in types)
            {
                result.AddRange(await _sut.ScanResources(type));
            }

            var containsDuplicates = result
                .DistinctBy(r => r.Key)
                .GroupBy(r => r.Key)
                .Any(g => g.Count() > 1);

            Assert.False(containsDuplicates);
        }

        [Fact]
        public async Task ModelWithCustomAttribute_DiscoversResource_PropertyName_As_Translation()
        {
            var resources = await _sut.ScanResources(typeof(ModelWithCustomAttributes));
            var helpTextResource = resources.First(r => r.PropertyName == "UserName-HelpText");

            Assert.Equal("UserName-HelpText", helpTextResource.Translations.DefaultTranslation());
        }

        [Fact]
        public async Task ModelWithSingleCustomAttribute_DiscoversBothResources()
        {
            var resources = await _sut.ScanResources(typeof(ModelWithSingleCustomAttribute));

            Assert.Equal(2, resources.Count());
        }

        [Fact]
        public void ModelWithDuplicateCustomAttribute_DoesNotThrowException()
        {
            var resources = _sut.ScanResources(typeof(ModelWithCustomAttributesDuplicates));

            Assert.NotNull(resources);
        }

        [Fact]
        public async Task CanSpecifyDefaultTranslation_UsingToStringOfAttribute()
        {
            var resources = await _sut.ScanResources(typeof(ModelWithCustomAttributeWithDefaultTranslation));

            Assert.NotNull(resources);

            var foreignResource = resources.First(r => r.PropertyName == "SomeProperty-WithDefaultTranslation");
            Assert.Equal("This is default translation", foreignResource.Translations.DefaultTranslation());
        }

        [Fact]
        public void SpecifyCustomAttributes_TargetIsNotAttribute_Exception()
        {
            var ctx = new ConfigurationContext();

            Assert.Throws<ArgumentException>(() =>
                ctx.CustomAttributes.Add<CustomAttributeScannerTests>());
        }
    }
}

