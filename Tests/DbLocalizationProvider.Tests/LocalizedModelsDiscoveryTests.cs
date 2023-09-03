using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedModelsDiscoveryTests: IAsyncLifetime
    {
        private readonly List<DiscoveredResource> _properties = new();
        private readonly TypeDiscoveryHelper _sut;

        public LocalizedModelsDiscoveryTests()
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
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);
        }

        public async Task InitializeAsync()
        {
            var types = new[] { typeof(SampleViewModel), typeof(SubViewModel) };
            foreach (var type in types)
            {
                _properties.AddRange(await _sut.ScanResources(type));
            }
        }

        public Task DisposeAsync() { return Task.CompletedTask; }

        [Fact]
        public void PropertyWithAttributes_DisplayDescription_Discovered()
        {
            var resource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.PropertyWithDescription");
            Assert.NotNull(resource);

            var propertyWithDescriptionResource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.PropertyWithDescription-Description");
            Assert.NotNull(propertyWithDescriptionResource);
        }

        [Fact]
        public void SingleLevel_ScalarProperties_NoAttribute()
        {
            var simpleProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty");
            Assert.NotNull(simpleProperty);

            var ignoredProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.IgnoredProperty");
            Assert.Null(ignoredProperty);

            Assert.Equal("SampleProperty", simpleProperty.Translations.DefaultTranslation());

            var simplePropertyWithDefaultValue = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty2");
            Assert.NotNull(simplePropertyWithDefaultValue);
            Assert.Equal("This is Display value", simplePropertyWithDefaultValue.Translations.DefaultTranslation());

            var simplePropertyRequired = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-Required");
            Assert.NotNull(simplePropertyRequired);
            Assert.Equal("SampleProperty-Required", simplePropertyRequired.Translations.DefaultTranslation());

            var simplePropertyStringLength = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-StringLength");
            Assert.NotNull(simplePropertyStringLength);

            var subProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SubViewModel.AnotherProperty");
            Assert.NotNull(subProperty);

            var includedSubProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.ComplexIncludedProperty");
            Assert.NotNull(includedSubProperty);

            var nullable = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.NullableInt");
            Assert.NotNull(nullable);
        }
    }
}
