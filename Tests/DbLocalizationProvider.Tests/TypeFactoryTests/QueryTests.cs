using Xunit;

namespace DbLocalizationProvider.Tests.TypeFactoryTests
{
    public class QueryTests
    {
        public QueryTests()
        {
            ConfigurationContext.Current.TypeFactory.ForQuery<SampleQuery>().SetHandler<SampleQueryHandler>();
        }

        [Fact]
        public void ExecuteQuery()
        {
            var q = new SampleQuery();

            var result = q.Execute();

            Assert.Equal("Sample string", result);
        }

        [Fact]
        public void ExecuteQuery_Decorated()
        {
            var sut = new TypeFactory();
            var query = new SampleQuery();

            sut.ForQuery<SampleQuery>().SetHandler<SampleQueryHandler>();
            sut.ForQuery<SampleQuery>().DecorateWith<DecoratedSampleQueryHandler>();

            var result = sut.GetQueryHandler(query).Execute(query);

            Assert.Equal("set from decorator", result);
        }
    }
}
