using DbLocalizationProvider.Queries;
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
    }
}
