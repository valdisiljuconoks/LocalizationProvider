using DbLocalizationProvider.Queries;
using Xunit;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class QueryTests
    {
        private readonly QueryExecutor _sut;

        public QueryTests()
        {
            var ctx = new ConfigurationContext();
            ctx.TypeFactory
                .ForQuery<DetermineDefaultCulture.Query>().SetHandler<DetermineDefaultCulture.Handler>()
                .ForQuery<SampleQuery>().SetHandler<SampleQueryHandler>();

            _sut = new QueryExecutor(ctx);
        }

        [Fact]
        public void ExecuteQuery()
        {
            var q = new SampleQuery();

            var result = _sut.Execute(q);

            Assert.Equal("Sample string", result);
        }

        [Fact]
        public void ExecuteQuery_Decorated()
        {
            var sut = new TypeFactory(new ConfigurationContext());
            var query = new SampleQuery();

            sut.ForQuery<SampleQuery>().SetHandler<SampleQueryHandler>();
            sut.ForQuery<SampleQuery>().DecorateWith<DecoratedSampleQueryHandler>();

            var result = sut.GetQueryHandler(query).Execute(query);

            Assert.Equal("set from decorator", result);
        }

        [Fact]
        public void DecoratedHandler_AdditionalConstructorParameters_ShouldBeAbleToCreate()
        {
            var sut = new TypeFactory(new ConfigurationContext
            {
                DiagnosticsEnabled = true
            });

            var query = new SampleQuery();

            sut.ForQuery<SampleQuery>().SetHandler<SampleQueryHandler>();
            sut.ForQuery<SampleQuery>().DecorateWith<DecoratedSampleQueryHandlerWithAdditionalArguments>();

            var result = sut.GetQueryHandler(query).Execute(query);

            Assert.Equal("set from decorator. from context: True", result);
        }

        [Fact]
        public void ReplaceRegisteredHandler_LatestShouldBeReturned()
        {
            var sut = new TypeFactory(new ConfigurationContext());
            sut.ForQuery<SampleQuery>().SetHandler<SampleQueryHandler>();

            var result = sut.GetHandler(typeof(SampleQuery));

            Assert.True(result is SampleQueryHandler);

            // replacing handler
            sut.ForQuery<SampleQuery>().SetHandler<AnotherSampleQueryHandler>();

            result = sut.GetHandler(typeof(SampleQuery));

            Assert.True(result is AnotherSampleQueryHandler);
        }
    }
}
