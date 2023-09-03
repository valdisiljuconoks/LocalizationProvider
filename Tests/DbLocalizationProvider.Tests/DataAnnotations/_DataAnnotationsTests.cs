using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DbLocalizationProvider.Abstractions;
using DbLocalizationProvider.Internal;
using DbLocalizationProvider.Queries;
using DbLocalizationProvider.Refactoring;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.DataAnnotations
{
    public class DataAnnotationsTests
    {
        private readonly LocalizationProvider _provider;
        private readonly TypeDiscoveryHelper _sut;

        public DataAnnotationsTests()
        {
            var state = new ScanState();
            var ctx = new ConfigurationContext();
            var keyBuilder = new ResourceKeyBuilder(state, ctx);
            var oldKeyBuilder = new OldResourceKeyBuilder(keyBuilder);
            ctx.TypeFactory
                .ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>()
                .ForQuery<GetTranslation.Query>().SetHandler<GetTranslationReturnResourceKeyHandler>();

            var queryExecutor = new QueryExecutor(ctx.TypeFactory);
            var translationBuilder = new DiscoveredTranslationBuilder(queryExecutor);

            _sut = new TypeDiscoveryHelper(new List<IResourceTypeScanner>
            {
                new LocalizedModelTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder),
                new LocalizedEnumTypeScanner(keyBuilder, translationBuilder),
                new LocalizedForeignResourceTypeScanner(keyBuilder, oldKeyBuilder, state, ctx, translationBuilder)
            }, ctx);

            var expressHelper = new ExpressionHelper(keyBuilder);
            _provider = new LocalizationProvider(keyBuilder, expressHelper, new FallbackLanguagesCollection(), queryExecutor);
        }

        [Fact]
        public async Task AdditionalCustomAttributesTest()
        {
            var result = await _provider.GetString(() => ResourceClassWithCustomAttributes.Resource1, typeof(CustomRegexAttribute));

            Assert.Equal("DbLocalizationProvider.Tests.DataAnnotations.ResourceClassWithCustomAttributes.Resource1-CustomRegex", result);
        }

        [Fact]
        public async Task ChildClassTypeAttributeUsage_ShouldRegisterResource()
        {
            var properties = await _sut.ScanResources(typeof(ViewModelWithInheritedDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Equal(2, properties.Count());
        }

        [Fact]
        public async Task DirectDataTypeAttributeUsage_ShouldNotRegisterResource()
        {
            var properties = await _sut.ScanResources(typeof(ViewModelWithSomeDataTypeAttributes));

            Assert.NotEmpty(properties);
            Assert.Single(properties);
        }
    }

    public class GetTranslationReturnResourceKeyHandler : IQueryHandler<GetTranslation.Query, string>
    {
        public Task<string> Execute(GetTranslation.Query query)
        {
            return Task.FromResult(query.Key);
        }
    }

    public class CustomRegexAttribute : RegularExpressionAttribute
    {
        public CustomRegexAttribute(string pattern) : base(pattern)
        {
        }
    }

    [LocalizedResource]
    public class ResourceClassWithCustomAttributes
    {
        [CustomRegex(".")]
        public static string Resource1 { get; set; } = "Resource 1";
    }
}
