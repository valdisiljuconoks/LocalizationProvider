using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void UninitializedCommand_ShouldThrowNiceException()
        {
            var command = new UpdateSchema.Command();
            command.Execute();
        }
    }
}
