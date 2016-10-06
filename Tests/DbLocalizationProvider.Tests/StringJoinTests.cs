using DbLocalizationProvider.Internal;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class StringJoinTests
    {
        [Fact]
        public void Join_MoreStrings_SomeEmpty()
        {
            var s = "Beginning";
            var result = s.JoinNonEmpty(".", "Ending");

            Assert.Equal("Beginning.Ending", result);
        }

        [Fact]
        public void Join_TwoNonEmptyStrings()
        {
            var s = "Beginning";
            var result = s.JoinNonEmpty(".", "Ending", "", "ReallyTheEnd");

            Assert.Equal("Beginning.Ending.ReallyTheEnd", result);
        }
    }
}
