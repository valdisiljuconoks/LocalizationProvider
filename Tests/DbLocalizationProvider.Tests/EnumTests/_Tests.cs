using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.EnumTests
{
    public class LocalizedEnumTests : IAsyncLifetime
    {
        private readonly List<DiscoveredResource> _properties = new();
        private readonly TypeDiscoveryHelper _sut;
        private readonly ExpressionHelper _expressionHelper;

        public LocalizedEnumTests()
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

            _expressionHelper = new ExpressionHelper(keyBuilder);
        }


        public async Task InitializeAsync()
        {
            var types = new[] { typeof(DocumentEntity) };
            Assert.NotEmpty(types);

            foreach (var type in types)
            {
                _properties.AddRange(await _sut.ScanResources(type));
            }
        }

        public Task DisposeAsync() { return Task.CompletedTask; }

        [Fact]
        public async Task DiscoverEnumValue_NameAsTranslation()
        {
            var properties = await _sut.ScanResources(typeof(SampleStatus));

            var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleStatus.Open");

            Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
        }

        [Fact]
        public async Task DiscoverEnumWithPrefixKey()
        {
            var properties = await _sut.ScanResources(typeof(SampleStatusWithPrefix));

            var openStatus = properties.First(p => p.Key == "ThisIsPrefix.Open");

            Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
        }

        [Fact]
        public void EnumType_CheckDiscovered_Found()
        {
            var enumProperty = _properties.Find(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.DocumentEntity.Status");
            Assert.NotNull(enumProperty);
        }

        [Fact]
        public async Task EnumWithDisplayAttribute_TranslationEqualToSpecifiedInAttribute()
        {
            var properties = await _sut.ScanResources(typeof(SampleEnumWithDisplayAttribute));

            var newStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleEnumWithDisplayAttribute.New");

            Assert.Equal("This is new", newStatus.Translations.DefaultTranslation());
        }

        [Fact]
        public async Task EnumWithAdditionalTranslation_DiscoversAllTranslations()
        {
            var properties = await _sut.ScanResources(typeof(SampleEnumWithAdditionalTranslations));

            var openStatus = properties.First(p => p.Key == "DbLocalizationProvider.Tests.EnumTests.SampleEnumWithAdditionalTranslations.Open");

            Assert.Equal(3, openStatus.Translations.Count);
            Assert.Equal("Open", openStatus.Translations.DefaultTranslation());
            Assert.Equal("Åpen", openStatus.Translations.First(t => t.Culture == "no").Translation);
        }

        [Fact]
        public async Task EnumWithResourceKeys_GeneratesKeysWithSpecifiedNames()
        {
            var discoveredResources = await _sut.ScanResources(typeof(SampleEnumWithKeys));

            Assert.NotEmpty(discoveredResources);

            Assert.Contains(discoveredResources, r => r.Key == "/this/is/key");
        }

        [Fact]
        public async Task EnumWithResourceKeys_GeneratesKeysWithSpecifiedNames_WithClassPrefix()
        {
            var discoveredResources = await _sut.ScanResources(typeof(SampleEnumWithKeysWithClassPrefix));

            Assert.NotEmpty(discoveredResources);

            Assert.Contains(discoveredResources, r => r.Key == "/this/is/prefix/and/this/is/key");
        }

        [Fact]
        public void Test_EnumExpression_AsProperty()
        {
            var doc = new DocumentEntity
            {
                Status = SampleStatus.New
            };

            Assert.Equal("DbLocalizationProvider.Tests.EnumTests.DocumentEntity.Status", _expressionHelper.GetFullMemberName(() => doc.Status));
        }

        [Fact]
        public void Test_EnumExpression_Directly()
        {
            Assert.Equal("DbLocalizationProvider.Tests.EnumTests.SampleStatus.Open", _expressionHelper.GetFullMemberName(() => SampleStatus.Open));
        }

        [Fact]
        public void Test_MemberAccessExpression()
        {
            var doc = new DocumentEntity();
            Assert.Equal("DbLocalizationProvider.Tests.EnumTests.DocumentEntity", _expressionHelper.GetFullMemberName(() => doc));
        }
    }
}
